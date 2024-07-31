using LoRinoBackend.Hubs; // Namespace for SignalR hub communication
using LoRinoBackend.Models; // Namespace for data models
using LoRinoBackend.ViewModels; // Namespace for view models
using Microsoft.AspNetCore.Authorization; // Namespace for authorization attributes
using Microsoft.AspNetCore.Hosting; // Namespace for web hosting
using Microsoft.AspNetCore.Mvc; // Namespace for MVC components
using Microsoft.AspNetCore.SignalR; // Namespace for SignalR functionality
using System; // Namespace for fundamental types and base classes
using System.Collections.Generic; // Namespace for collection types
using System.IO; // Namespace for input/output operations
using System.Linq; // Namespace for LINQ operations
using System.Threading.Tasks; // Namespace for asynchronous programming

namespace LoRinoBackend.Controllers
{
    public class DeviceTypeController : Controller
    {
        // Dependency injection for required services
        private readonly IDeviceRepository deviceRepository;
        private readonly IDeviceTypeRepository deviceTypeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        // Constructor to initialize dependencies
        public DeviceTypeController(IHubContext<WanesyHub> hubContext, // Hub context for SignalR communication
                                    IDeviceRepository deviceRepository, // Repository for device data
                                    IWebHostEnvironment hostingEnvironment, // Hosting environment details
                                    IDeviceTypeRepository deviceTypeRepository) // Repository for device type data
        {
            this.deviceRepository = deviceRepository; // Assigns device repository
            this.deviceTypeRepository = deviceTypeRepository; // Assigns device type repository
            this.hostingEnvironment = hostingEnvironment; // Assigns hosting environment
        }

        // Processes uploaded file and returns the unique file name
        private string ProcessUploadedFile(DeviceCreateTypeViewModel model)
        {
            string uniqueFileName = null; // Initialize unique file name variable
            if (model.Photo != null) // Check if photo is uploaded
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "devices"); // Define the upload folder path
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName; // Create a unique file name
                string filePath = Path.Combine(uploadsFolder, uniqueFileName); // Define the file path
                using (var fileStream = new FileStream(filePath, FileMode.Create)) // Create a file stream to save the file
                {
                    model.Photo.CopyTo(fileStream); // Copy the uploaded photo to the file stream
                }
            }
            return uniqueFileName; // Return the unique file name
        }

        // Returns error information as JSON
        public IActionResult returnError(int errorCode, string errorData)
        {
            return Json(new ReturnError { ErrorCode = errorCode, ErrorDescription = errorData }); // Return error data as JSON
        }

        // Retrieves all device types data as JSON
        public IActionResult GetAllData(string Id, string s)
        {
            var model = deviceTypeRepository.GetAllData(); // Fetch all device types
            return Json(model); // Return data as JSON
        }

        // Retrieves the last device type data as JSON
        public IActionResult GetLastData(string Id, string s)
        {
            var model = deviceTypeRepository.GetLastData(); // Fetch the latest device type data
            return Json(model); // Return data as JSON
        }

        // Displays a view with all device types
        [AllowAnonymous] // Allow access without authentication
        public ViewResult Index()
        {
            var model = deviceTypeRepository.GetAllDeviceType(); // Fetch all device types
            return View(model); // Return the view with the list of device types
        }

        // Displays the view for creating a new device type
        [HttpGet] // Specifies that this action handles GET requests
        public ViewResult Create()
        {
            return View(); // Return the view for creating a new device type
        }

        // Handles POST request to create a new device type
        [HttpPost] // Specifies that this action handles POST requests
        public async Task<IActionResult> Create(DeviceType device)
        {
            deviceTypeRepository.Add(device); // Add the new device type to the repository
            this.HttpContext.Response.StatusCode = 201; // Set the response status code to 201 (Created)
            return Json(device); // Return the created device type as JSON
        }

        // Displays details for a specific device type
        public ViewResult Details(int id)
        {
            DeviceType deviceType = deviceTypeRepository.GetDeviceType(id); // Retrieve the device type with the specified ID

            // Handle case where device type is not found
            if (deviceType == null)
            {
                Response.StatusCode = 404; // Set the response status code to 404 (Not Found)
                return View("DeviceNotFound", id); // Return the "DeviceNotFound" view with the device type ID
            }

            var deviceDetailsTypeViewModel = new DeviceDetailsTypeViewModel() // Create a new DeviceDetailsTypeViewModel
            {
                DeviceType = deviceType, // Set the device type
                PageTitle = "Device Details" // Set the page title
            };

            return View(deviceDetailsTypeViewModel); // Return the view with the device details model
        }

        // Displays the view for editing a device type
        [HttpGet] // Specifies that this action handles GET requests
        [Authorize] // Requires user to be authorized
        public ViewResult Edit(int Id)
        {
            DeviceType deviceType = deviceTypeRepository.GetDeviceType(Id); // Retrieve the device type with the specified ID
            var deviceEditTypeViewModel = new DeviceEditTypeViewModel // Create a new DeviceEditTypeViewModel
            {
                Id = deviceType.Id, // Set the device type ID
                Name = deviceType.Name, // Set the device type name
                ExistingPhotoPath = deviceType.PhotoPath // Set the existing photo path
            };
            return View(deviceEditTypeViewModel); // Return the edit view with the model
        }

        // Handles POST request to update a device type
        [HttpPost] // Specifies that this action handles POST requests
        [Authorize] // Requires user to be authorized
        public IActionResult Edit(DeviceEditTypeViewModel model)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                DeviceType deviceType = deviceTypeRepository.GetDeviceType(model.Id); // Retrieve the device type with the specified ID

                deviceType.Name = model.Name; // Update the device type name

                // Handle photo update
                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null) // Check if there is an existing photo
                    {
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "devices", model.ExistingPhotoPath); // Define the file path
                        System.IO.File.Delete(filePath); // Delete the existing photo
                    }
                    deviceType.PhotoPath = ProcessUploadedFile(model); // Upload and set the new photo path
                }

                deviceTypeRepository.Update(deviceType); // Update the device type in the repository

                return RedirectToAction("Details", new { id = deviceType.Id }); // Redirect to the device type details view
            }
            return View(); // Return the view with the model in case of invalid state
        }

        // Handles POST request to delete a device type's photo
        [HttpPost] // Specifies that this action handles POST requests
        [Authorize(Policy = "DeleteRolePolicy")] // Requires user to have the "DeleteRolePolicy" policy
        public IActionResult DeletePhoto(int id)
        {
            DeviceType deviceType = deviceTypeRepository.GetDeviceType(id); // Retrieve the device type with the specified ID
            if (deviceType.PhotoPath != null) // Check if the device type has a photo
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "devices", deviceType.PhotoPath); // Define the file path
                System.IO.File.Delete(filePath); // Delete the photo
                deviceType.PhotoPath = null; // Set the photo path to null
                deviceTypeRepository.Update(deviceType); // Update the device type in the repository
            }
            return RedirectToAction("Details", new { id = deviceType.Id }); // Redirect to the device type details view
        }
    }
}
