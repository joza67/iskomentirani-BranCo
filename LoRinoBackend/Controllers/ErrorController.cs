using System; // Provides fundamental classes and base classes
using System.Collections.Generic; // Provides classes for collections
using System.Linq; // Provides functionality to query data structures
using System.Threading.Tasks; // Provides types for asynchronous operations
using Microsoft.AspNetCore.Authorization; // Provides classes for authorization
using Microsoft.AspNetCore.Diagnostics; // Provides classes for diagnostic information
using Microsoft.AspNetCore.Mvc; // Provides classes for building MVC applications
using Microsoft.Extensions.Logging; // Provides classes for logging

// Controller for handling error responses and displaying error views
namespace LoRinoBackend.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger; // Logger instance for logging error details

        // Constructor to initialize the logger
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger; // Assigns the injected logger to the logger field
        }

        // Handles HTTP status codes and logs relevant information
        [Route("Error/{statusCode}")] // Route attribute to handle specific error status codes
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            // Retrieves details of the original request that resulted in the status code
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // Determines the appropriate error message and logging based on the status code
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found"; // Sets error message for 404 status code
                    logger.LogWarning($"404 Error Occurred. Path = {statusCodeResult.OriginalPath}" + // Logs the original path
                        $" and QueryString = {statusCodeResult.OriginalQueryString}"); // Logs the original query string
                    break;
                    // Add additional cases here for other status codes if needed
            }

            // Returns the "NotFound" view for 404 errors
            return View("NotFound");
        }

        // Handles general errors and logs the exception details
        [Route("Error")] // Route attribute to handle general errors
        [AllowAnonymous] // Allows access to the error page without authentication
        public IActionResult Error()
        {
            // Retrieves details of the exception that occurred
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // Logs the exception details
            logger.LogError($"The path {exceptionDetails.Path} threw an exception " + // Logs the path where the exception occurred
                $"{exceptionDetails.Error}"); // Logs the exception details

            // Returns the "Error" view for general errors
            return View("Error");
        }
    }
}
