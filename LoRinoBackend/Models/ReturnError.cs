// Importing necessary namespaces for core functionalities
using System; // Provides fundamental types and functions
using System.Collections.Generic; // Provides collection types such as lists and dictionaries
using System.Linq; // Provides LINQ functionality for querying collections
using System.Threading.Tasks; // Provides support for asynchronous programming

namespace LoRinoBackend.Models
{
    // Class representing an error return structure
    // It is used to encapsulate error information that can be returned from methods or APIs
    public class ReturnError
    {
        // Property to hold the error code
        // An integer representing the specific error
        public int ErrorCode { get; set; }

        // Property to hold a description of the error
        // A string providing a human-readable explanation of the error
        public string ErrorDescription { get; set; }
    }
}
