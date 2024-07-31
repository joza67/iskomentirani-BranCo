using System; // Importing the System namespace
using System.Collections.Generic; // Importing the namespace for generic collections
using System.Linq; // Importing the namespace for Linq
using System.Threading.Tasks; // Importing the namespace for asynchronous programming
using LoRinoBackend.Models; // Importing the namespace for Models
using Microsoft.AspNetCore.Mvc; // Importing the namespace for MVC

namespace LoRinoBackend.Controllers
{
    // Controller for handling test-related actions
    public class TestController : Controller
    {
        // Action method to render the Index view
        public IActionResult Index()
        {
            return View(); // Returns the Index view to the client
        }

        // Action method to handle GET requests for creating LoRa data
        [HttpGet]
        public IActionResult Create([FromBody] LoRaData loraData)
        {
            // The [FromBody] attribute indicates that the parameter 'loraData'
            // is expected to be bound from the body of the HTTP request.
            // Note: For GET requests, request body is generally not used. This might be a design issue.

            return View(); // Returns the Create view to the client
        }
    }
}
