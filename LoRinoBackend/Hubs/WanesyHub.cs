using System; // Importing base class library, including fundamental classes and base classes that define commonly-used value and reference data types
using System.Collections.Generic; // Importing collections, including data structures such as lists and dictionaries
using System.Linq; // Importing LINQ (Language-Integrated Query) features for querying collections
using Microsoft.AspNetCore.SignalR; // Importing ASP.NET Core SignalR, a library for adding real-time web functionality
using System.Threading.Tasks; // Importing support for asynchronous programming
using LoRinoBackend.Models; // Importing models from the LoRinoBackend project

namespace LoRinoBackend.Hubs
{
    public class WanesyHub : Hub // Defining a SignalR Hub class named WanesyHub
    {
        public static List<string> Users = new List<string>(); // Static list to store connected user identifiers

        // Method to send a message to all connected clients
        //public async Task SendMessage(string device, string message, double unixTimeStamp, int fPort, int eventId, int locationId, int alarmCount, int _retId)
        //{
        //    await Clients.All.SendAsync("WanesyNotify", device, message, unixTimeStamp, fPort, eventId, locationId, alarmCount, _retId);
        //}

        // Method to send a message to all connected clients with specified parameters
        public async Task SendMessage(int id, long eventCreationTime, int alarmCount, int locationId, string guid)
        {
            await Clients.All.SendAsync("WanesyNotify", id, eventCreationTime, alarmCount, locationId, guid); // Sending message to all clients with the specified parameters
        }
    }
}
