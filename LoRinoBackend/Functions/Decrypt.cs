using LoRinoBackend.Services; // Importing necessary services from the LoRinoBackend.Services namespace
using System; // Importing base class library, including fundamental classes and base classes that define commonly-used value and reference data types
using System.Security.Cryptography; // Importing cryptographic services, including secure encoding and decoding of data

namespace LoRinoBackend.Functions
{
    ///<summary>
    /// Class for decryption
    ///</summary>
    public class Decrypt
    {
        ///<summary>
        /// Function for generating a cryptographic salt.
        /// Takes the device's DevEUI and port as byte variables.
        /// Returns a byte array iv.
        ///</summary>
        // What is salt and IV in encryption?
        // Salt is essential to prevent precomputation attacks.
        // IV (or nonce with counter modes) ensures that the same plaintext produces different ciphertexts.
        // Prevents attackers from using patterns in the plaintext to gather information from a set of encrypted messages.
        public static byte[] GenerateSalt(string devEui, byte port)
        {
            byte[] newDevEui = HexString.HexStringToByte(devEui); // Convert the hexadecimal string of DevEUI to a byte array
            byte[] iv = new byte[16]; // Initialize a byte array iv with a length of 16

            Array.Reverse(newDevEui); // Reverse the byte array newDevEui

            Array.Copy(newDevEui, 0, iv, 0, 8); // Copy the first 8 bytes of the reversed newDevEui into iv starting at index 0

            iv[8] = Convert.ToByte(port); // Set the 9th byte of iv to the byte value of the port

            return iv; // Return the iv byte array
        }
        ///<summary>
        /// Function for generating a cryptographic salt.
        /// Takes the message as a byte array ([]data), keys as a byte array ([]keyHash), and cryptographic salt as a byte array ([]iv).
        /// Returns the decrypted message in the form of a byte array plainText.
        ///</summary>
        // What is AES-128 encryption?
        // AES-128 uses a 128-bit key length for encrypting and decrypting message blocks.
        // AES-192 uses a 192-bit key length for encrypting and decrypting message blocks.
        // AES-256 uses a 256-bit key length for encrypting and decrypting message blocks.
        public static byte[] Aes128Decrypt(byte[] data, byte[] keyHash, byte[] iv)
        {
            // Commented out code showing a previous implementation
            // byte[] decrypted = new byte[data.Length];
            // Aes128CounterMode am = new Aes128CounterMode(iv);
            // ICryptoTransform ict = am.CreateDecryptor(keyHash, null);
            // ict.TransformBlock(data, 0, data.Length, decrypted, 0);
            // return decrypted;

            byte[] plainText = new byte[data.Length]; // Initialize a byte array plainText with the same length as data
            Aes128CounterMode am = new Aes128CounterMode(iv); // Create an instance of Aes128CounterMode with iv
            ICryptoTransform ict = am.CreateDecryptor(keyHash, null); // Create a decryptor using keyHash and iv
            ict.TransformBlock(data, 0, data.Length, plainText, 0); // Decrypt the data and store the result in plainText
            return plainText; // Return the decrypted plainText byte array
        }
    }
}
