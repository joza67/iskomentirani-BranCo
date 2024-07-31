// Import necessary namespaces
using System; // Provides fundamental classes and types
using System.Collections.Generic; // Provides classes for working with collections
using System.Linq; // Provides LINQ (Language Integrated Query) functionality
using System.Threading.Tasks; // Provides types for asynchronous programming

namespace LoRinoBackend.Models
{
    // Interface defining a contract for a client that handles messages
    interface IWanesyClient
    {
        // Asynchronous method for receiving a message from a device
        // Parameters:
        // - device: A string representing the device sending the message
        // - message: A string containing the message to be received
        // Returns a Task, allowing for asynchronous operation
        Task ReceiveMessage(string device, string message);
    }
}
