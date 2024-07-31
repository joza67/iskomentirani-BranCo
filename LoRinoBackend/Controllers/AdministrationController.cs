using LoRinoBackend.Models; // Import the models from the LoRinoBackend project
using LoRinoBackend.ViewModels; // Import the view models from the LoRinoBackend project
using Microsoft.AspNetCore.Authorization; // Import authorization attributes
using Microsoft.AspNetCore.Identity; // Import identity management classes
using Microsoft.AspNetCore.Mvc; // Import MVC controller and action result classes
using Microsoft.EntityFrameworkCore; // Import Entity Framework Core for database interactions
using System; // Import basic types and base class functionalities
using System.Collections.Generic; // Import collection types like List
using System.IO; // Import file input/output functionalities
using System.Linq; // Import LINQ functionalities for querying
using System.Security.Claims; // Import Claims for identity management
using System.Threading.Tasks; // Import asynchronous programming functionalities
using Microsoft.AspNetCore.Hosting; // Import hosting environment functionalities
using System.Collections; // Import collection types like Hashtable
using Microsoft.AspNetCore.Mvc.Rendering; // Import functionalities for rendering HTML elements in MVC views
using System.Net.Mail; // Import SMTP functionalities for sending emails
using System.Net; // Import functionalities for IP and other network functions
using Microsoft.AspNetCore; // Import ASP.NET Core functionalities

namespace LoRinoBackend.Controllers // Declare the namespace for the controller
{
    [Authorize] // Indicates that the controller requires authorization
    public class AdministrationController : Controller // Define the AdministrationController class that inherits from Controller
    {
        // Define private fields for the services
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICompanyRepository companyRepository;
        private readonly ILocationRepository locationRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IAlarmSoundRepository alarmSoundRepository;

        // Constructor to initialize the dependencies through dependency injection
        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ICompanyRepository companyRepository, IWebHostEnvironment hostingEnvironment, ILocationRepository locationRepository, IAlarmSoundRepository alarmSoundRepository)
        {
            this.roleManager = roleManager; // Initialize RoleManager
            this.userManager = userManager; // Initialize UserManager
            this.companyRepository = companyRepository; // Initialize CompanyRepository
            this.locationRepository = locationRepository; // Initialize LocationRepository
            this.hostingEnvironment = hostingEnvironment; // Initialize HostingEnvironment
            this.alarmSoundRepository = alarmSoundRepository; // Initialize AlarmSoundRepository
        }

        // Displays the Access Denied page
        [HttpGet] // Indicates that this action method handles GET requests
        [AllowAnonymous] // Allows access to this method without authentication
        public IActionResult AccessDenied()
        {
            return View(); // Returns the default view for Access Denied
        }

        // Displays a list of users for SuperAdmin
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult ListUsers()
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            ApplicationUser user = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == userId).Result; // Retrieve the current user including the company info

            var isSuperAdmin = true; // Placeholder for checking if the user is a Super Admin
            var companyId = user.Company.Id; // Get the company ID of the current user

            List<ApplicationUser> users;

            if (!isSuperAdmin) // If the user is not a Super Admin
            {
                users = userManager.Users.Include(e => e.Company).Where(i => i.Company.Id == companyId).ToList(); // Get users belonging to the same company
            }
            else
            {
                users = userManager.Users.Include(e => e.Company).ToList(); // Get all users
            }
            return View(users); // Return the list of users to the view
        }

        // Displays a list of companies for SuperAdmin
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult ListCompanies()
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            var companies = companyRepository.GetAllCompanies(); // Get all companies
            return View(companies); // Return the list of companies to the view
        }

        // Displays the Create Company page for SuperAdmin
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult CreateCompany()
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            return View(); // Return the default view for CreateCompany
        }

        // Handles the Create Company form submission for SuperAdmin
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult CreateCompany(CompanyCreateViewModel model)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                string uniqueFileName = ProcessUploadedFile(model); // Process the uploaded file
                Company newCompany = new Company
                {
                    Name = model.Name, // Set the company name
                    Email = model.Email, // Set the company email
                    Street = model.Street, // Set the company street
                    City = model.City, // Set the company city
                    Country = model.Country, // Set the company country
                    PhotoPath = uniqueFileName // Set the company photo path
                };

                companyRepository.Add(newCompany); // Add the new company to the repository

                return RedirectToAction("CompanyDetails", new { id = newCompany.Id }); // Redirect to the CompanyDetails view
            }
            return View(); // Return the default view if model state is invalid
        }

        // Helper method to process uploaded company photo
        private string ProcessUploadedFile(CompanyCreateViewModel model)
        {
            string uniqueFileName = null; // Initialize unique file name
            if (model.Photo != null) // Check if a photo was uploaded
            {
                string uploadsFolder = Path.Combine(this.hostingEnvironment.WebRootPath, "images", "company"); // Set the uploads folder path
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName; // Generate a unique file name
                string filePath = Path.Combine(uploadsFolder, uniqueFileName); // Set the file path
                using (var fileStream = new FileStream(filePath, FileMode.Create)) // Create a new file stream
                {
                    model.Photo.CopyTo(fileStream); // Copy the uploaded photo to the file stream
                }
            }
            return uniqueFileName; // Return the unique file name
        }

        // Displays details of a specific company for SuperAdmin
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public ViewResult CompanyDetails(int id)
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            Company company = companyRepository.GetCompany(id); // Get the company by ID

            if (company == null) // Check if the company was not found
            {
                Response.StatusCode = 404; // Set the response status code to 404
                return View("Company NotFound", id); // Return the "Company NotFound" view
            }

            CompanyDetailsViewModel companyDetailsViewModel = new CompanyDetailsViewModel()
            {
                Company = company, // Set the company
                PageTitle = "Company Details" // Set the page title
            };

            return View(companyDetailsViewModel); // Return the company details view model to the view
        }

        // Displays the Company Edit page for SuperAdmin
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public ViewResult CompanyEdit(int Id)
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            Company company = companyRepository.GetCompany(Id); // Get the company by ID
            CompanyEditViewModel companyEditViewModel = new CompanyEditViewModel
            {
                Id = company.Id, // Set the company ID
                Name = company.Name, // Set the company name
                Street = company.Street, // Set the company street
                City = company.City, // Set the company city
                Country = company.Country, // Set the company country
                Email = company.Email, // Set the company email
                ExistingPhotoPath = company.PhotoPath // Set the existing photo path
            };
            return View(companyEditViewModel); // Return the company edit view model to the view
        }

        // Handles the Company Edit form submission for SuperAdmin
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult CompanyEdit(CompanyEditViewModel model)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                Company company = companyRepository.GetCompany(model.Id); // Get the company by ID

                company.Name = model.Name; // Update the company name
                company.Email = model.Email; // Update the company email
                company.Street = model.Street; // Update the company street
                company.City = model.City; // Update the company city
                company.Country = model.Country; // Update the company country

                if (model.Photo != null) // Check if a new photo was uploaded
                {
                    if (model.ExistingPhotoPath != null) // Check if there is an existing photo
                    {
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "company", model.ExistingPhotoPath); // Set the existing file path
                        System.IO.File.Delete(filePath); // Delete the existing file
                    }
                    company.PhotoPath = ProcessUploadedFile(model); // Process the uploaded photo and set the new photo path
                }

                companyRepository.Update(company); // Update the company in the repository

                return RedirectToAction("CompanyDetails", new { id = company.Id }); // Redirect to the CompanyDetails view
            }
            return View(); // Return the default view if model state is invalid
        }

        // Handles the Company Delete action for SuperAdmin
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult CompanyDelete(int id)
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            Company company = companyRepository.GetCompany(id); // Get the company by ID
            companyRepository.Delete(id); // Delete the company from the repository
            if (company.PhotoPath != null) // Check if there is a photo path
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "company", company.PhotoPath); // Set the file path
                System.IO.File.Delete(filePath); // Delete the photo file
                company.PhotoPath = null; // Set the photo path to null
            }
            return RedirectToAction("ListCompanies"); // Redirect to the ListCompanies view
        }

        // Handles the Delete Company Photo action for SuperAdmin
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult DeleteCompanyPhoto(int id)
        {
            Company company = companyRepository.GetCompany(id); // Get the company by ID
            if (company.PhotoPath != null) // Check if there is a photo path
            {
                string uploadsFolder = Path.Combine(this.hostingEnvironment.WebRootPath, "images", "company", company.PhotoPath); // Set the uploads folder path
                System.IO.File.Delete(uploadsFolder); // Delete the photo file
                company.PhotoPath = null; // Set the photo path to null
                companyRepository.Update(company); // Update the company in the repository
            }
            return RedirectToAction("CompanyDetails", new { id = company.Id }); // Redirect to the CompanyDetails view
        }

        // Display a list of roles for SuperAdmin
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public IActionResult ListRoles()
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            var roles = roleManager.Roles; // Get all roles
            return View(roles); // Return the list of roles to the view
        }

        // Display the Edit Role page for SuperAdmin
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> EditRole(string id)
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            var role = await roleManager.FindByIdAsync(id); // Find the role by ID

            if (role == null) // Check if the role was not found
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            var model = new RoleEditViewModel
            {
                Id = role.Id, // Set the role ID
                RoleName = role.Name // Set the role name
            };

            foreach (var user in userManager.Users) // Retrieve all the users
            {
                if (await userManager.IsInRoleAsync(user, role.Name)) // Check if the user is in this role
                {
                    model.Users.Add(user.UserName); // Add the username to the Users list in the model
                }
            }

            return View(model); // Return the role edit view model to the view
        }

        // Handles the Edit Role form submission for SuperAdmin
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> EditRole(RoleEditViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id); // Find the role by ID

            if (role == null) // Check if the role was not found
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }
            else
            {
                role.Name = model.RoleName; // Update the role name

                var result = await roleManager.UpdateAsync(role); // Update the role in the database

                if (result.Succeeded) // Check if the update was successful
                {
                    return RedirectToAction("ListRoles"); // Redirect to the ListRoles view
                }

                foreach (var error in result.Errors) // Display validation errors if update fails
                {
                    ModelState.AddModelError("", error.Description); // Add each error to the model state
                }

                return View(model); // Return the model to the view if update fails
            }
        }

        // Displays the Edit Users in Role page for SuperAdmin
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var userId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate alarm sound for the user
            ViewBag.roleId = roleId; // Set the role ID in the ViewBag

            var role = await roleManager.FindByIdAsync(roleId); // Find the role by ID

            if (role == null) // Check if the role was not found
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            var model = new List<RoleUserViewModel>();

            foreach (var user in userManager.Users) // Retrieve all the users
            {
                var userRoleViewModel = new RoleUserViewModel
                {
                    UserId = user.Id, // Set the user ID
                    UserName = user.UserName // Set the username
                };

                if (await userManager.IsInRoleAsync(user, role.Name)) // Check if the user is in this role
                {
                    userRoleViewModel.IsSelected = true; // Set IsSelected to true
                }
                else
                {
                    userRoleViewModel.IsSelected = false; // Set IsSelected to false
                }

                model.Add(userRoleViewModel); // Add the user role view model to the model
            }

            return View(model); // Return the list of users in role to the view
        }

        // Handles the Edit Users in Role form submission for SuperAdmin
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> EditUsersInRole(List<RoleUserViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId); // Find the role by ID

            if (role == null) // Check if the role was not found
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            for (int i = 0; i < model.Count; i++) // Iterate through each user in the model
            {
                var user = await userManager.FindByIdAsync(model[i].UserId); // Find the user by ID

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name))) // Check if the user is selected and not already in the role
                {
                    result = await userManager.AddToRoleAsync(user, role.Name); // Add the user to the role
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name)) // Check if the user is not selected and is already in the role
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name); // Remove the user from the role
                }
                else
                {
                    continue; // Continue to the next iteration if no changes are needed
                }

                if (result.Succeeded) // Check if the operation was successful
                {
                    if (i < (model.Count - 1))
                        continue; // Continue to the next iteration if not the last user
                    else
                        return RedirectToAction("EditRole", new { Id = roleId }); // Redirect to the EditRole view
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId }); // Redirect to the EditRole view
        }

        // Displays the Edit User page for SuperAdmin
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> EditUser(string id)
        {
            var currentUserId = userManager.GetUserId(HttpContext.User); // Get the current user's ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(currentUserId); // Activate alarm sound for the current user
            ApplicationUser currentUser = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == currentUserId).Result; // Retrieve the current user including the company info

            var isSuperAdmin = userManager.IsInRoleAsync(currentUser, "Super Admin").Result; // Check if the current user is a Super Admin
            var currentUserCompanyId = currentUser.Company.Id; // Get the company ID of the current user

            var user = await userManager.FindByIdAsync(id); // Find the user by ID

            if (user == null) // Check if the user was not found
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            var userClaims = await userManager.GetClaimsAsync(user); // Get the list of user claims
            var userRoles = await userManager.GetRolesAsync(user); // Get the list of user roles

            RegisterViewModel rm = new RegisterViewModel(); // Initialize a new RegisterViewModel

            rm.CompanyId = currentUserCompanyId; // Set the company ID for the current user
            var userlocations = (from cu in locationRepository.GetAllLocationUsers()
                                 where cu.UserId == id
                                 select new LocationUser
                                 {
                                     Id = cu.Id,
                                     UserId = cu.UserId,
                                     UserName = (from u in userManager.Users where u.Id == cu.UserId select u.UserName).FirstOrDefault(),
                                     LocationId = cu.LocationId,
                                     LocationName = (from c in locationRepository.GetAllLocations() where c.Id == cu.LocationId select c.Name).FirstOrDefault()
                                 }).ToList(); // Get the locations associated with the user

            var model = new UserEditViewModel
            {
                Id = user.Id, // Set the user ID
                FirstName = user.FirstName, // Set the first name
                LastName = user.LastName, // Set the last name
                Email = user.Email, // Set the email
                PhoneNumber = user.PhoneNumber, // Set the phone number
                UserName = user.UserName, // Set the username
                Street = user.Streeet, // Set the street
                City = user.City, // Set the city
                Country = user.Country, // Set the country
                CompanyList = companyRepository.GetAllCompanies()
                                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                .ToList(), // Get the list of all companies
                CompanyId = user.Company.Id, // Set the company ID
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(), // Set the user claims
                Roles = userRoles, // Set the user roles
                UserLocations = userlocations // Set the user locations
            };

            return View(model); // Return the user edit view model to the view
        }

        // Endpoint to handle the submission of user edits
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> EditUser(UserEditViewModel model)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                var user = await userManager.FindByIdAsync(model.Id); // Find the user by ID

                if (user == null) // Check if the user was not found
                {
                    ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found"; // Set the error message
                    return View("NotFound"); // Return the NotFound view
                }
                else
                {
                    Company cp = companyRepository.GetCompany(model.CompanyId); // Retrieve the company by the provided company ID

                    user.FirstName = model.FirstName; // Update the user's first name
                    user.LastName = model.LastName; // Update the user's last name
                    user.Email = model.Email; // Update the user's email
                    user.PhoneNumber = model.PhoneNumber; // Update the user's phone number
                    user.UserName = model.UserName; // Update the user's username
                    user.Company = cp; // Update the user's company
                    user.Streeet = model.Street; // Update the user's street
                    user.City = model.City; // Update the user's city
                    user.Country = model.Country; // Update the user's country

                    var result = await userManager.UpdateAsync(user); // Update the user in the database

                    if (result.Succeeded) // Check if the update was successful
                    {
                        return RedirectToAction("UserDetails", new { id = model.Id }); // Redirect to the UserDetails view
                    }

                    foreach (var error in result.Errors) // Display validation errors if update fails
                    {
                        ModelState.AddModelError("", error.Description); // Add each error to the model state
                    }
                }
            }

            model.CompanyList = companyRepository.GetAllCompanies()
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToList(); // Populate the list of companies for the view

            return View(model); // Return the model to the view if update fails
        }

        // Generates the body of the email for email confirmation
        private string mailBodyConfirmEmail(string email, string firstName, string link)
        {
            string body = string.Empty; // Initialize an empty string for the body

            using (StreamReader reader = new StreamReader("./wwwroot/mailTemplateConfirmEmail.html")) // Read the HTML template from the file
            {
                body = reader.ReadToEnd(); // Set the body to the contents of the file
            }

            body = body.Replace("{username}", email); // Replace the username placeholder with the actual username
            body = body.Replace("{firstName}", firstName); // Replace the firstName placeholder with the actual first name
            body = body.Replace("{resetLink}", link); // Replace the resetLink placeholder with the actual link

            return body; // Return the body
        }

        // Endpoint to resend confirmation email
        [HttpPost] // Indicates that this action method handles POST requests
        [AllowAnonymous] // Allows access to this method without authentication
        public async Task<IActionResult> SendMailAgain(string id)
        {
            var user = await userManager.FindByIdAsync(id); // Find the user by ID
            if (user != null) // Check if the user exists
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user); // Generate an email confirmation token

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme); // Create a confirmation link

                try
                {
                    var smtp = new SmtpClient("mail.microlink.hr", 465) // Configure the SMTP client
                    {
                        Credentials = new NetworkCredential("iot", "LdcZea7GV%+Gpa~w]*Y6eMmvZ;U'LR"), // Set the SMTP credentials
                        EnableSsl = false, // Disable SSL
                    };

                    MailMessage message = new MailMessage // Create a new email message
                    {
                        From = new MailAddress("iot@microlink.hr") // Set the sender's email address
                    };
                    message.To.Add(user.Email); // Add the user's email address as the recipient
                    message.Subject = "Potvrda računa"; // Set the subject of the email
                    message.IsBodyHtml = true; // Set the email body format to HTML
                    message.Body = mailBodyConfirmEmail(user.Email, user.FirstName, confirmationLink); // Set the body of the email

                    string userState = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8); // Generate a unique user state for the email

                    smtp.SendAsync(message, userState); // Send the email asynchronously
                }
                catch
                {
                    // Handle exceptions silently
                }

                ViewBag.Title = "Registracija uspješna"; // Set the view title
                ViewBag.Message = "Prije prijave molimo potvrdite svoj račun klikom na poveznicu koju smo poslali na Vašu e-poštu."; // Set the view message
            }

            return RedirectToAction("ListUsers"); // Redirect to the ListUsers view
        }

        // Endpoint to handle the deletion of a user
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id); // Find the user by ID

            if (user == null) // Check if the user was not found
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }
            else
            {
                var roles = await userManager.GetRolesAsync(user); // Get the user's roles
                var resultRole = await userManager.RemoveFromRolesAsync(user, roles); // Remove the user's roles

                if (!resultRole.Succeeded) // Check if the roles removal was unsuccessful
                {
                    ViewBag.ErrorTitle = "Error"; // Set the error title
                    ViewBag.ErrorMessage = $"Cannot remove roles from User with Id = {id}"; // Set the error message
                    return View("Error"); // Return the Error view
                }

                var claims = await userManager.GetClaimsAsync(user); // Get the user's claims
                var resultClaim = await userManager.RemoveClaimsAsync(user, claims); // Remove the user's claims

                if (!resultClaim.Succeeded) // Check if the claims removal was unsuccessful
                {
                    ViewBag.ErrorTitle = "Error"; // Set the error title
                    ViewBag.ErrorMessage = $"Cannot remove claims from User with Id = {id}"; // Set the error message
                    return View("Error"); // Return the Error view
                }

                locationRepository.DeleteLocationUsers(id); // Delete the user's associated location users

                var result = await userManager.DeleteAsync(user); // Delete the user

                if (result.Succeeded) // Check if the deletion was successful
                {
                    ViewBag.Deleted = $"Korisnik {user.FirstName} {user.LastName} ({user.Email}) je izbrisan."; // Set the success message
                    return RedirectToAction("ListUsers"); // Redirect to the ListUsers view
                }

                foreach (var error in result.Errors) // Display validation errors if deletion fails
                {
                    ModelState.AddModelError("", error.Description); // Add each error to the model state
                }

                return View("ListUsers"); // Return the ListUsers view
            }
        }

        // Endpoint to get details of a user
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "AdminRolePolicy")] // Restricts access to users with the "AdminRolePolicy" policy
        public async Task<IActionResult> UserDetails(string id)
        {
            var currentUserId = id; // Set the current user's ID

            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(currentUserId); // Activate sound alarm for the current user

            ApplicationUser currentUser = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == currentUserId).Result; // Retrieve the current user including the company info

            var isSuperAdmin = userManager.IsInRoleAsync(currentUser, "Super Admin").Result; // Check if the current user is a Super Admin
            var currentUserCompanyId = currentUser.Company.Id; // Get the company ID of the current user

            var user = await userManager.FindByIdAsync(id); // Find the user by ID

            if (user == null) // Check if the user was not found
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            var userClaims = await userManager.GetClaimsAsync(user); // Get the user's claims
            var userRoles = await userManager.GetRolesAsync(user); // Get the user's roles

            RegisterViewModel rm = new RegisterViewModel // Initialize a new RegisterViewModel
            {
                CompanyId = currentUserCompanyId // Set the company ID
            };

            var model = new UserDetailsViewModel
            {
                Id = user.Id, // Set the user ID
                FirstName = user.FirstName, // Set the first name
                LastName = user.LastName, // Set the last name
                Email = user.Email, // Set the email
                PhoneNumber = user.PhoneNumber, // Set the phone number
                UserName = user.UserName, // Set the username
                Street = user.Streeet, // Set the street
                City = user.City, // Set the city
                Country = user.Country, // Set the country
                Company = user.Company, // Set the company
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(), // Set the claims
                Roles = userRoles // Set the roles
            };

            return View(model); // Return the user details view model to the view
        }

        // Endpoint to get the current user's profile
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize] // Requires authorization
        public IActionResult Profile()
        {
            var currentUserId = userManager.GetUserId(HttpContext.User); // Get the ID of the currently logged-in user

            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(currentUserId); // Activate sound alarm for the current user

            ApplicationUser currentUser = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == currentUserId).Result; // Retrieve the current user including the company info

            return View(currentUser); // Return the view with the current user's profile
        }

        // Endpoint to handle the deletion of a role
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id); // Find the role by ID

            if (role == null) // Check if the role was not found
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role); // Delete the role

                    if (result.Succeeded) // Check if the deletion was successful
                    {
                        return RedirectToAction("ListRoles"); // Redirect to the ListRoles view
                    }

                    foreach (var error in result.Errors) // Display validation errors if deletion fails
                    {
                        ModelState.AddModelError("", error.Description); // Add each error to the model state
                    }

                    return View("ListRoles"); // Return the ListRoles view if deletion fails
                }
                catch (DbUpdateException ex) // Handle exception if the role is in use
                {
                    ViewBag.ErrorTitle = $"{role.Name} role is in use. " + ex.Message; // Set the error title
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete"; // Set the error message
                    return View("Error"); // Return the Error view
                }
            }
        }

        // Endpoint to manage user roles
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId; // Set the user ID in the ViewBag
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate sound alarm for the user

            var user = await userManager.FindByIdAsync(userId); // Find the user by ID

            if (user == null) // Check if the user was not found
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in roleManager.Roles) // Iterate over all roles and create view models
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id, // Set the role ID
                    RoleName = role.Name // Set the role name
                };

                model.Add(userRolesViewModel); // Add the user roles view model to the list
            }

            foreach (var role in model.OrderBy(x => x.RoleId)) // Set the IsSelected property based on whether the user has the role
            {
                if (await userManager.IsInRoleAsync(user, role.RoleName)) // Check if the user has the role
                {
                    role.IsSelected = true; // Set IsSelected to true
                }
                else
                {
                    role.IsSelected = false; // Set IsSelected to false
                }
            }

            return View(model); // Return the list of roles to the view
        }

        // Endpoint to handle changes in user roles
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId); // Find the user by ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate sound alarm for the user

            if (user == null) // Check if the user was not found
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            var roles = await userManager.GetRolesAsync(user); // Get the user's roles
            var result = await userManager.RemoveFromRolesAsync(user, roles); // Remove all existing roles from the user

            if (!result.Succeeded) // Check if the removal was unsuccessful
            {
                ModelState.AddModelError("", "Cannot remove user existing roles"); // Add error to the model state
                return View(model); // Return the model to the view
            }

            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName)); // Add the selected roles to the user

            if (!result.Succeeded) // Check if adding roles was unsuccessful
            {
                ModelState.AddModelError("", "Cannot add selected roles to user"); // Add error to the model state
                return View(model); // Return the model to the view
            }

            return RedirectToAction("EditUser", new { Id = userId }); // Redirect to the EditUser view
        }

        // Endpoint to manage user claims
        [HttpGet] // Indicates that this action method handles GET requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId); // Find the user by ID
            ViewBag.Alarm = alarmSoundRepository.ActivateSoundAlarm(userId); // Activate sound alarm for the user

            if (user == null) // Check if the user was not found
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user); // Retrieve existing user claims

            var model = new UserClaimsViewModel
            {
                UserId = userId // Set the user ID
            };

            foreach (Claim claim in ClaimsStore.AllClaims) // Loop through all possible claims and create user claim view models
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type // Set the claim type
                };

                userClaim.IsSelected = existingUserClaims.Any(c => String.Equals(c.Type, claim.Type, StringComparison.OrdinalIgnoreCase) && String.Equals(c.Value, "true")); // Set IsSelected property

                model.Cliams.Add(userClaim); // Add the user claim to the model
            }

            return View(model); // Return the view with the claims model
        }

        // Endpoint to handle changes in user claims
        [HttpPost] // Indicates that this action method handles POST requests
        [Authorize(Policy = "SuperAdmin")] // Restricts access to users with the "SuperAdmin" policy
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId); // Find the user by ID

            if (user == null) // Check if the user was not found
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found"; // Set the error message
                return View("NotFound"); // Return the NotFound view
            }

            var claims = await userManager.GetClaimsAsync(user); // Retrieve existing claims
            var result = await userManager.RemoveClaimsAsync(user, claims); // Remove all existing claims from the user

            if (!result.Succeeded) // Check if the removal was unsuccessful
            {
                ModelState.AddModelError("", "Cannot remove user existing claims"); // Add error to the model state
                return View(model); // Return the model to the view
            }

            result = await userManager.AddClaimsAsync(user, model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false"))); // Add the selected claims to the user

            if (!result.Succeeded) // Check if adding claims was unsuccessful
            {
                ModelState.AddModelError("", "Cannot add selected claims to user"); // Add error to the model state
                return View(model); // Return the model to the view
            }

            return RedirectToAction("EditUser", new { Id = model.UserId }); // Redirect to the EditUser view
        }

    }
}
