using System; // Importing base class library, including fundamental classes and base classes that define commonly-used value and reference data types

namespace LoRinoBackend.Functions
{
    ///<summary>
    /// Class for handling strings in Hex format
    ///</summary>
    public static class HexString
    {
        ///<summary>
        /// Method for converting HexString to bytes.
        /// Takes a hexadecimal representation as a string variable.
        /// Returns a byte array.
        ///</summary>
        public static byte[] HexStringToByte(this string hex)
        {
            int len = hex.Length; // Get the length of the hex string
            byte[] bytes = new byte[len / 2]; // Create a byte array with half the length of the hex string

            for (int i = 0; len > i; i += 2) // Loop through the hex string, two characters at a time
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16); // Convert each pair of hex characters to a byte
            }

            return bytes; // Return the byte array
        }

        ///<summary>
        /// Method for converting bytes to HexString.
        /// Takes a byte array.
        /// Returns a hexadecimal representation as a string variable.
        ///</summary>
        public static string ByteToHexString(this byte[] data)
        {
            string ret = String.Empty; // Initialize an empty string

            foreach (byte b in data) // Loop through each byte in the byte array
            {
                ret += b.ToString("x2"); // Convert the byte to a two-character hexadecimal string and append it to the result
            }
            return ret; // Return the resulting hex string
        }

        ///<summary>
        /// Method for checking if a specific bit is set in a byte.
        /// Takes a byte and the position of the bit.
        /// Returns a boolean indicating if the bit at the specified position is set.
        ///</summary>
        public static bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0; // Check if the bit at position 'pos' in byte 'b' is set
        }
    }
}
