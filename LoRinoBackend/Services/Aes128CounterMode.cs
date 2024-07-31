using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace LoRinoBackend.Services
{
    // Custom implementation of AES encryption using Counter Mode
    public class Aes128CounterMode : SymmetricAlgorithm
    {
        private readonly byte[] _counter;  // Counter value used in Counter Mode
        private readonly AesManaged _aes;  // Instance of AES algorithm with ECB mode

        // Constructor to initialize the AES algorithm and set the counter
        public Aes128CounterMode(byte[] counter)
        {
            if (counter == null) throw new ArgumentNullException("counter"); // Ensure counter is not null
            if (counter.Length != 16) // Check if counter size is correct (AES block size is 16 bytes)
                throw new ArgumentException(String.Format("Counter size must be same as block size (actual: {0}, expected: {1})",
                    counter.Length, 16));

            _aes = new AesManaged
            {
                Mode = CipherMode.ECB,  // ECB mode is used for Counter Mode encryption
                Padding = PaddingMode.None  // No padding is used
            };

            _counter = counter;
        }

        // Create encryptor for Counter Mode
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] ignoredParameter)
        {
            return new CounterModeCryptoTransform(_aes, rgbKey, _counter);
        }

        // Create decryptor for Counter Mode
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] ignoredParameter)
        {
            return new CounterModeCryptoTransform(_aes, rgbKey, _counter);
        }

        // Generate a key (not used directly in Counter Mode)
        public override void GenerateKey()
        {
            _aes.GenerateKey();
        }

        // Generate an IV (not needed for Counter Mode, but method is required by base class)
        public override void GenerateIV()
        {
            // IV not needed in Counter Mode
            _aes.GenerateIV();
        }
    }

    // Custom implementation of ICryptoTransform for AES Counter Mode
    public class CounterModeCryptoTransform : ICryptoTransform
    {
        private readonly byte[] _counter;  // Counter value used in Counter Mode
        private readonly ICryptoTransform _counterEncryptor;  // Encryptor for the counter
        private readonly Queue<byte> _xorMask = new Queue<byte>();  // Queue to store XOR mask bytes
        private readonly SymmetricAlgorithm _symmetricAlgorithm;  // Instance of the symmetric algorithm

        // Constructor to initialize the CounterModeCryptoTransform
        public CounterModeCryptoTransform(SymmetricAlgorithm symmetricAlgorithm, byte[] key, byte[] counter)
        {
            if (symmetricAlgorithm == null) throw new ArgumentNullException("symmetricAlgorithm");
            if (key == null) throw new ArgumentNullException("key");
            if (counter == null) throw new ArgumentNullException("counter");
            if (counter.Length != symmetricAlgorithm.BlockSize / 8) // Ensure counter size matches block size
                throw new ArgumentException(String.Format("Counter size must be same as block size (actual: {0}, expected: {1})",
                    counter.Length, symmetricAlgorithm.BlockSize / 8));

            _symmetricAlgorithm = symmetricAlgorithm;
            _counter = counter;

            var zeroIv = new byte[_symmetricAlgorithm.BlockSize / 8]; // Zero IV for encryption
            _counterEncryptor = symmetricAlgorithm.CreateEncryptor(key, zeroIv);
        }

        // Transform the final block of data
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var output = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        // Transform a block of data
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            for (var i = 0; i < inputCount; i++)
            {
                if (NeedMoreXorMaskBytes()) EncryptCounterThenIncrement(); // Ensure XOR mask is available

                var mask = _xorMask.Dequeue(); // Get the XOR mask byte
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ mask); // Apply XOR mask
            }

            return inputCount;
        }

        // Check if more XOR mask bytes are needed
        private bool NeedMoreXorMaskBytes()
        {
            return _xorMask.Count == 0;
        }

        // Encrypt the counter and increment it for the next block
        private void EncryptCounterThenIncrement()
        {
            var counterModeBlock = new byte[_symmetricAlgorithm.BlockSize / 8];

            _counterEncryptor.TransformBlock(_counter, 0, _counter.Length, counterModeBlock, 0); // Encrypt counter
            IncrementCounter(); // Increment the counter

            foreach (var b in counterModeBlock)
            {
                _xorMask.Enqueue(b); // Add encrypted counter bytes to the XOR mask queue
            }
        }

        // Increment the counter value
        private void IncrementCounter()
        {
            for (var i = _counter.Length - 1; i >= 0; i--)
            {
                if (++_counter[i] != 0)
                    break; // Stop incrementing when there is no overflow
            }
        }

        public int InputBlockSize { get { return _symmetricAlgorithm.BlockSize / 8; } } // Size of input block
        public int OutputBlockSize { get { return _symmetricAlgorithm.BlockSize / 8; } } // Size of output block
        public bool CanTransformMultipleBlocks { get { return true; } } // Multiple blocks can be transformed
        public bool CanReuseTransform { get { return false; } } // Transform cannot be reused

        public void Dispose()
        {
        }
    }
}
