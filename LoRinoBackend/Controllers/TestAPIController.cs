using Microsoft.AspNetCore.Mvc; // Importing the namespace for MVC
using System.IO; // Importing the namespace for I/O operations
using System.Net; // Importing the namespace for networking
using System; // Importing the System namespace
using LoRinoBackend.Models; // Importing the namespace for Models
using Newtonsoft.Json; // Importing the namespace for JSON operations
using System.Collections.Generic; // Importing the namespace for generic collections
using System.Linq; // Importing the namespace for Linq
using System.Net.Http; // Importing the namespace for HTTP client operations
using System.Text; // Importing the namespace for text operations
using System.Threading.Tasks; // Importing the namespace for asynchronous programming
using Microsoft.AspNetCore.Http; // Importing the namespace for HTTP context
using System.Diagnostics; // Importing the namespace for debugging

namespace LoRinoBackend.Controllers
{
    // Controller for handling test API operations
    public class TestAPIController : Controller
    {
        private readonly IDeviceRepository _deviceRepository; // Repository for device operations
        private readonly ILocationRepository _locationRepository; // Repository for location operations

        // Constructor to inject dependencies
        public TestAPIController(IDeviceRepository deviceRepository, ILocationRepository locationRepository)
        {
            _deviceRepository = deviceRepository;
            _locationRepository = locationRepository;
        }

        // Action method to render the index view
        public IActionResult Index()
        {
            // Get the current time in UTC and convert it to Unix time milliseconds
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();

            // Populate ViewBag with a list of devices for dropdown
            ViewBag.Devices = _deviceRepository.DevicesForDropDownList();

            return View();
        }

        // Action method to handle POST request for scrambling test data
        [HttpPost]
        public void ScrambleTest()
        {
            // Get the current time in UTC and convert it to Unix time milliseconds
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();

            var payload = "d91b04fe7afb4afcb2aa"; // Test payload data
            var url = "http://localhost:44376/Home/Create"; // URL to send POST request
            Random r = new Random();
            int rInt = r.Next(1, 2); // Random integer for testing

            // Get a list of locations and select a random one
            var random = new Random();
            List<Location> locationList = _locationRepository.GetAllLocations().ToList();
            int index = random.Next(locationList.Count) + 1;
            Location location = locationList.FirstOrDefault(a => a.Id == index);

            // Get a list of devices for the selected location and randomly select a few
            List<Device> deviceList = _deviceRepository.GetAllData().Where(a => a.LocationId == location.Id).ToList();
            var sortedList = deviceList.OrderBy(x => r.Next()).Take(rInt);

            // Iterate through the selected devices and send POST requests
            foreach (var device in sortedList)
            {
                var httpClient = new HttpClient(); // Create an HTTP client
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                // Create the data payload for the POST request
                var data = "{\'endDevice\':{\'devEui\':\'" + device.DevEui + "\',\'cluster\':{}},\'payload\':\'" + payload + "\',\'recvTime\':" + unixTimeMilliseconds + ",\'gwInfo\':[{}]}'";

                // Send the POST request and log the result
                Debug.WriteLine(PostAsync(url, data));
            }
        }

        // Asynchronous method to send POST requests and handle responses
        public async Task<IActionResult> PostAsync(string url, string content)
        {
            var httpClient = new HttpClient(); // Create an HTTP client

            // Create the content to send with the POST request
            var contenta = new StringContent(content, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                // Send the POST request asynchronously
                HttpResponseMessage response = await httpClient.PostAsync(url, contenta);

                if (response.IsSuccessStatusCode)
                {
                    // Read and return the response content if the request is successful
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response: " + responseContent);
                    return Ok(responseContent);
                }
                else
                {
                    // Log and return the error status if the request fails
                    Console.WriteLine("HTTP Request Failed: " + response.StatusCode);
                    return BadRequest(response.StatusCode);
                }
            }
            catch (HttpRequestException e)
            {
                // Log and return the exception message if an HTTP request exception occurs
                Console.WriteLine("HTTP Request Exception: " + e.Message);
                return BadRequest(e.Message);
            }
        }

        // Action method to send data with a POST request and redirect to the index view
        [HttpPost]
        public IActionResult Send(int id, long recvTime)
        {
            // Get the current time in UTC and convert it to Unix time milliseconds
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            var payload = "d91b04fe7afb4afcb2aa"; // Test payload data

            // Get the devEui from the device repository using the provided ID
            string devEui = _deviceRepository.GetDevEuiFromId(id);

            // Set recvTime to the current time if it's zero
            if (recvTime != 0)
            {
                recvTime = unixTimeMilliseconds;
            }

            var url = "http://localhost:44376/Home/Create"; // URL to send POST request

            // Create an HTTP request and configure it
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";

            // Create the data payload for the POST request
            var data = "{\'endDevice\':{\'devEui\':\'" + devEui + "\',\'cluster\':{}},\'payload\':\'" + payload + "\',\'recvTime\':" + recvTime + ",\'gwInfo\':[{}]}'";

            // Write the data payload to the request stream
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            // Get and log the response from the request
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            Console.WriteLine(httpResponse.StatusCode);
            return RedirectToAction(nameof(Index));
        }
    }
}
