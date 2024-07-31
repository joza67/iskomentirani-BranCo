using LoRinoBackend.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoRinoBackend.Services
{
    public static class Configuring
    {
        public static long ExpiredDate { get; set; } = 0;  // Timestamp when the token expires
        public static string Token { get; set; } = "";    // Authentication token

        // DTO (Data Transfer Object) for DataDown requests
        public class DataDownDto
        {
            public int fPort { get; set; }                  // Port for the data
            public bool confirmed { get; set; }             // Whether the message is confirmed
            public string contentType { get; set; }         // Content type of the payload
            public endDevice endDevice { get; set; }         // End device information
            public string payload { get; set; }              // Payload to be sent
        }

        // DTO for end device information
        public class endDevice
        {
            public string devEui { get; set; }  // Device EUI (Extended Unique Identifier)
        }

        // Asynchronous method to obtain a token
        public static async Task<string> GetToken()
        {
            // JSON payload for the authentication request
            string cont = "{\"login\":\"superadmin\",\"password\":\"wmcAdmin13245\"}";

            // Create a new HttpClient instance
            using (var httpClient = new HttpClient())
            {
                // Create an HTTP POST request
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://wmc.lora.hr/gms/application/login"))
                {
                    request.Headers.TryAddWithoutValidation("accept", "application/json");  // Accept JSON responses
                    request.Content = new StringContent(cont);  // Set the request content
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.kerlink.iot-v1+json");  // Set content type

                    // Debugging output
                    Debug.WriteLine(request.Content);
                    Debug.Write(request);

                    // Send the request and ensure the response is successful
                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    // Asynchronously read the response content as a byte array
                    var output = response.Content.ReadAsByteArrayAsync();

                    // Parse the response JSON and extract the token and expiry date
                    var jsonDoc = JsonDocument.Parse(await output);
                    var result = jsonDoc.RootElement.GetProperty("token").ToString();
                    var expiry = jsonDoc.RootElement.GetProperty("expiredDate").ToString();
                    ExpiredDate = long.Parse(expiry);  // Update the token expiry date
                    return result;  // Return the token
                }
            }
        }

        // Asynchronous method to send data to a device
        [HttpPost]
        public static async Task<ObjectResult> SendDataToDevice(string payload, string dev, int port)
        {
            var result = new ObjectResult(new { currentDate = DateTime.Now });  // Initialize result object with current date

            // Check if payload or device ID is null
            if (payload == null || dev == null)
            {
                result.StatusCode = 406;  // Not acceptable
                return result;
            }

            // Check if the token is expired or not set
            if (ExpiredDate <= Unix.ToUnixTimeStamp(DateTime.Now) || ExpiredDate == 0)
            {
                Token = await GetToken();  // Get a new token
                result.StatusCode = 401;  // Unauthorized
                return result;
            }

            // Create the end device and message DTOs
            endDevice device = new endDevice { devEui = dev };
            var mess = new DataDownDto { fPort = port, confirmed = false, contentType = "HEXA", endDevice = device, payload = payload };

            // Serialize the message DTO to JSON
            var message = JsonConvert.SerializeObject(mess);
            var content = new StringContent(message.ToString(), Encoding.UTF8, "application/json");

            try
            {
                // Create a new HttpClient instance
                using (var httpClient = new HttpClient())
                {
                    // Create an HTTP POST request
                    using (var request = new HttpRequestMessage(HttpMethod.Post, "https://wmc.lora.hr/gms/application/dataDown"))
                    {
                        request.Headers.TryAddWithoutValidation("accept", "application/json");  // Accept JSON responses
                        request.Headers.TryAddWithoutValidation("Authorization", Token);  // Add authorization header
                        request.Content = new StringContent(message, Encoding.UTF8, "application/json");  // Set request content
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.kerlink.iot-v1+json");  // Set content type
                        request.Headers.Add("Contet.Length", message.Length.ToString());  // Add content length header

                        // Debugging output
                        Debug.WriteLine(request.Content);
                        Debug.Write(request);

                        // Send the request and ensure the response is successful
                        var response = await httpClient.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        result.StatusCode = (int?)response.StatusCode;  // Set the status code of the result
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = StatusCodes.Status500InternalServerError;  // Internal server error
                result.Value = "Failed to send " + ex;  // Set the error message
                return result;
            }
        }
    }
}
