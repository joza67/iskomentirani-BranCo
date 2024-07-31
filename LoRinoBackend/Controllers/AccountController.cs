using System; // Provides basic types and functionalities such as Int32, String, DateTime, etc.
using System.Collections.Generic; // Provides classes for defining generic collections like List<T>.
using System.IO; // Provides types for working with file and data streams.
using System.Linq; // Provides LINQ query capabilities on collections.
using System.Net; // Provides classes for network communication, including HTTP.
using System.Net.Http.Headers; // Provides support for manipulating HTTP headers.
using System.Net.Mail; // Provides classes for sending email.
using System.Threading.Tasks; // Provides types for working with asynchronous operations.
using LoRinoBackend.Models; // Imports models defined in the LoRinoBackend application.
using LoRinoBackend.ViewModels; // Imports view models for data binding in views.
using Microsoft.AspNetCore.Authorization; // Provides attributes for authorizing user access.
using Microsoft.AspNetCore.Identity; // Provides classes for user authentication and management.
using Microsoft.AspNetCore.Mvc; // Provides classes for building MVC web applications.
using Microsoft.AspNetCore.Mvc.Rendering; // Provides classes for rendering HTML elements.
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core functionalities.
using Microsoft.Extensions.Logging; // Provides logging functionalities.
using Newtonsoft.Json; // Provides JSON serialization and deserialization functionalities.

namespace LoRinoBackend.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICompanyRepository companyRepository; // Repository for accessing company data.
        private readonly ILocationRepository locationRepository; // Repository for accessing location data.
        private readonly ILogger<AccountController> logger; // Logger for logging information.
        private readonly SignInManager<ApplicationUser> signInManager; // Manages user sign-in.
        private readonly UserManager<ApplicationUser> userManager; // Manages user operations.

        // Constructor that initializes dependencies via dependency injection.
        public AccountController(ICompanyRepository companyRepository, ILocationRepository locationRepository, ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.companyRepository = companyRepository; // Initializes company repository.
            this.locationRepository = locationRepository; // Initializes location repository.
            this.logger = logger; // Initializes logger.
            this.signInManager = signInManager; // Initializes sign-in manager.
            this.userManager = userManager; // Initializes user manager.
        }

        // Logs the user out and redirects to the login page.
        [HttpPost] // Indicates that this action method handles HTTP POST requests.
        [AllowAnonymous] // Allows access without authentication.
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // Signs the user out.
            return RedirectToAction("Login", "Account"); // Redirects to the Login action in the Account controller.
        }

        // Checks if the provided email is already in use.
        [AcceptVerbs("Get", "Post")] // Specifies that this method can handle both GET and POST requests.
        [AllowAnonymous] // Allows access without authentication.
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email); // Finds a user by email.
            if (user == null)
            {
                return Json(true); // Returns true if the email is not in use.
            }
            else
            {
                return Json($"E-pošta {email} se već koristi."); // Returns a message if the email is already in use.
            }
        }

        // Displays a test page for admin users (e.g., after successful registration).
        [HttpGet] // Indicates that this action method handles HTTP GET requests.
        [Authorize(Policy = "AdminRolePolicy")] // Requires the user to meet the AdminRolePolicy authorization policy.
        public IActionResult Test()
        {
            ViewBag.Title = "Registracija uspješna"; // Sets the title for the view.
            ViewBag.Message = "Prije prijave molimo potvrdite svoj račun klikom na poveznicu koju smo poslali na Vašu e-poštu."; // Sets a confirmation message.
            return View("RegisterConfirmation"); // Returns the RegisterConfirmation view.
        }

        // Renders the registration page with company list for selection.
        [HttpGet] // Indicates that this action method handles HTTP GET requests.
        [Authorize(Policy = "AdminRolePolicy")] // Requires the user to meet the AdminRolePolicy authorization policy.
        public IActionResult Register()
        {
            var currentUserId = userManager.GetUserId(HttpContext.User); // Retrieves the current user's ID.

            ApplicationUser currentUser = userManager.Users.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == currentUserId).Result; // Retrieves the current user's details, including the company.
            var isSuperAdmin = userManager.IsInRoleAsync(currentUser, "Super Admin").Result; // Checks if the user is a Super Admin.
            var currentUserCompanyId = currentUser.Company.Id; // Retrieves the ID of the user's company.

            RegisterViewModel rm = new RegisterViewModel
            {
                CompanyList = companyRepository.GetAllCompanies()
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToList(), // Retrieves and sets the list of companies for selection.
                CompanyId = currentUserCompanyId // Sets the current user's company ID.
            };

            return View(rm); // Returns the Register view with the RegisterViewModel.
        }

        // Handles the registration of a new user.
        [HttpPost] // Indicates that this action method handles HTTP POST requests.
        [AllowAnonymous] // Allows access without authentication.
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid) // Checks if the model state is valid.
            {
                Company cp = companyRepository.GetCompany(model.CompanyId); // Retrieves the selected company.
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName, // Sets the user's first name.
                    LastName = model.LastName, // Sets the user's last name.
                    UserName = model.Email, // Sets the user's username as their email.
                    Email = model.Email, // Sets the user's email.
                    Company = cp // Sets the user's company.
                };

                // Creates the user and generates an email confirmation token.
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded) // Checks if the user creation was successful.
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user); // Generates an email confirmation token.
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme); // Generates a confirmation link.

                    // Sends a confirmation email.
                    try
                    {
                        var smtp = new SmtpClient("mail.microlink.hr", 465)
                        {
                            Credentials = new NetworkCredential("iot", "LdcZea7GV%+Gpa~w]*Y6eMmvZ;U'LR"), // Sets SMTP server credentials.
                            EnableSsl = false, // Disables SSL.
                        };

                        MailMessage message = new()
                        {
                            From = new MailAddress("iot@microlink.hr"), // Sets the sender's email address.
                            To = { user.Email }, // Sets the recipient's email address.
                            Subject = "Potvrda računa", // Sets the email subject.
                            IsBodyHtml = true, // Indicates that the body of the email is HTML.
                            Body = mailBodyConfirmEmail(user.Email, user.FirstName, confirmationLink) // Sets the email body.
                        };

                        string userState = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8); // Generates a unique state identifier.
                        smtp.SendAsync(message, userState); // Sends the email asynchronously.
                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation(ex.Message); // Logs any exceptions that occur.
                    }

                    ViewBag.Title = "Registracija uspješna"; // Sets the view title.
                    ViewBag.Message = "Prije prijave molimo potvrdite svoj račun klikom na poveznicu koju smo poslali na Vašu e-poštu."; // Sets a confirmation message.
                    return View("RegisterConfirmation"); // Returns the RegisterConfirmation view.
                }

                // Displays validation errors if registration fails.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description); // Adds each error to the model state.
                }
            }

            model.CompanyList = companyRepository.GetAllCompanies()
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList(); // Retrieves and sets the list of companies for selection.

            return View(model); // Returns the Register view with the updated model.
        }

        // Confirms the user's email using the provided user ID and token.
        [AllowAnonymous] // Allows access without authentication.
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) // Checks if the userId or token is null.
            {
                return RedirectToAction("Index", "Employee"); // Redirects to the Index action in the Employee controller.
            }

            var user = await userManager.FindByIdAsync(userId); // Finds the user by ID.
            if (user == null) // Checks if the user exists.
            {
                ViewBag.ErrorMessage = $"Korisnik {userId} je nevaljan"; // Sets an error message if the user is not found.
                return View("NotFound"); // Returns the NotFound view.
            }

            var result = await userManager.ConfirmEmailAsync(user, token); // Confirms the user's email.
            if (result.Succeeded) // Checks if the email confirmation was successful.
            {
                return View(); // Returns the default view.
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed"; // Sets an error title.
            return View("Error"); // Returns the Error view.
        }

        // Displays the login page.
        [HttpGet] // Indicates that this action method handles HTTP GET requests.
        [AllowAnonymous] // Allows access without authentication.
        public IActionResult Login()
        {
            return View(); // Returns the Login view.
        }

        // Handles the login process.
        [HttpPost] // Indicates that this action method handles HTTP POST requests.
        [AllowAnonymous] // Allows access without authentication.
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid) // Checks if the model state is valid.
            {
                var user = await userManager.FindByEmailAsync(model.Email); // Finds the user by email.

                if (user != null && !user.EmailConfirmed && // Checks if the email is not confirmed.
                    await userManager.CheckPasswordAsync(user, model.Password)) // Checks if the password is correct.
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user); // Generates an email confirmation token.
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme); // Generates a confirmation link.
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet"); // Adds an error message to the model state.
                    return View(model); // Returns the Login view with the model.
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true); // Signs in the user.
                if (result.Succeeded) // Checks if the sign-in was successful.
                {
                    if (!string.IsNullOrEmpty(returnUrl)) // Checks if a return URL is provided.
                    {
                        return LocalRedirect(returnUrl); // Redirects to the specified return URL.
                    }
                    else
                    {
                        return RedirectToAction("Index", "MV"); // Redirects to the Index action in the MV controller.
                    }
                }

                if (result.IsLockedOut) // Checks if the user account is locked out.
                {
                    return View("AccountLocked"); // Returns the AccountLocked view.
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt"); // Adds an error message for invalid login attempts.
            }
            return View(model); // Returns the Login view with the model.
        }

        // Displays the Forgot Password page.
        [HttpGet] // Indicates that this action method handles HTTP GET requests.
        [AllowAnonymous] // Allows access without authentication.
        public IActionResult ForgotPassword()
        {
            return View(); // Returns the ForgotPassword view.
        }

        // Handles Forgot Password requests.
        [HttpPost] // Indicates that this action method handles HTTP POST requests.
        [AllowAnonymous] // Allows access without authentication.
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid) // Checks if the model state is valid.
            {
                var user = await userManager.FindByEmailAsync(model.Email); // Finds the user by email.
                if (user != null && await userManager.IsEmailConfirmedAsync(user)) // Checks if the email is confirmed.
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user); // Generates a password reset token.
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme); // Generates a password reset link.

                    ViewBag.ErrorTitle = "Reset password link successful"; // Sets an error title.

                    var smtp = new SmtpClient("mail.microlink.hr", 465)
                    {
                        Credentials = new NetworkCredential("iot", "LdcZea7GV%+Gpa~w]*Y6eMmvZ;U'LR"), // Sets SMTP server credentials.
                        EnableSsl = false, // Disables SSL.
                    };

                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress("iot@microlink.hr"), // Sets the sender's email address.
                        To = { model.Email }, // Sets the recipient's email address.
                        Subject = "Zaboravljena lozinka", // Sets the email subject.
                        IsBodyHtml = true, // Indicates that the body of the email is HTML.
                        Body = mailBodyResetPassword(model.Email, user.FirstName, passwordResetLink) // Sets the email body.
                    };

                    string userState = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8); // Generates a unique state identifier.
                    smtp.SendAsync(message, userState); // Sends the email asynchronously.
                }

                return View("ForgotPasswordConfirmation"); // Returns the ForgotPasswordConfirmation view.
            }

            return View("ForgotPasswordConfirmation"); // Returns the ForgotPasswordConfirmation view.
        }

        // Displays the Reset Password page.
        [HttpGet] // Indicates that this action method handles HTTP GET requests.
        [AllowAnonymous] // Allows access without authentication.
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null) // Checks if the token or email is null.
            {
                ModelState.AddModelError("", "Invalid password reset token"); // Adds an error message to the model state.
            }
            return View(); // Returns the ResetPassword view.
        }

        // Handles the Reset Password process.
        [HttpPost] // Indicates that this action method handles HTTP POST requests.
        [AllowAnonymous] // Allows access without authentication.
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid) // Checks if the model state is valid.
            {
                var user = await userManager.FindByEmailAsync(model.Email); // Finds the user by email.

                if (user != null) // Checks if the user exists.
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password); // Resets the user's password.
                    if (result.Succeeded) // Checks if the password reset was successful.
                    {
                        if (await userManager.IsLockedOutAsync(user)) // Checks if the user account is locked out.
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow); // Unlocks the user account.
                        }
                        return View("ResetPasswordConfirmation"); // Returns the ResetPasswordConfirmation view.
                    }

                    foreach (var error in result.Errors) // Iterates over each error.
                    {
                        ModelState.AddModelError("", error.Description); // Adds each error to the model state.
                    }
                    return View(model); // Returns the ResetPassword view with the model.
                }

                return View("ResetPasswordConfirmation"); // Returns the ResetPasswordConfirmation view.
            }
            return View(model); // Returns the ResetPassword view with the model.
        }

        // Displays the Change Password page.
        [HttpGet] // Indicates that this action method handles HTTP GET requests.
        public IActionResult ChangePassword()
        {
            return View(); // Returns the ChangePassword view.
        }

        // Handles the Change Password process.
        [HttpPost] // Indicates that this action method handles HTTP POST requests.
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid) // Checks if the model state is valid.
            {
                var user = await userManager.GetUserAsync(User); // Retrieves the currently signed-in user.
                if (user == null) // Checks if the user is null.
                {
                    return RedirectToAction("Login"); // Redirects to the Login action.
                }

                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword); // Changes the user's password.
                if (!result.Succeeded) // Checks if the password change was not successful.
                {
                    foreach (var error in result.Errors) // Iterates over each error.
                    {
                        ModelState.AddModelError(string.Empty, error.Description); // Adds each error to the model state.
                    }
                    return View(); // Returns the ChangePassword view with errors.
                }

                await signInManager.RefreshSignInAsync(user); // Refreshes the sign-in session.
                return View("ChangePasswordConfirmation"); // Returns the ChangePasswordConfirmation view.
            }
            return View(model); // Returns the ChangePassword view with the model.
        }

        // Displays the Access Denied page.
        [HttpGet] // Indicates that this action method handles HTTP GET requests.
        [AllowAnonymous] // Allows access without authentication.
        public IActionResult AccessDenied()
        {
            return View(); // Returns the AccessDenied view.
        }

        // Helper method to generate the body of the email confirmation message.
        private string mailBodyConfirmEmail(string email, string firstName, string link)
        {
            string body = string.Empty; // Initializes the body as an empty string.
            using (StreamReader reader = new StreamReader("./wwwroot/mailTemplateConfirmEmail.html")) // Opens the email template file.
            {
                body = reader.ReadToEnd(); // Reads the content of the file.
            }
            body = body.Replace("{username}", email); // Replaces the placeholder with the email.
            body = body.Replace("{firstName}", firstName); // Replaces the placeholder with the first name.
            body = body.Replace("{resetLink}", link); // Replaces the placeholder with the confirmation link.
            return body; // Returns the formatted email body.
        }

        // Helper method to generate the body of the password reset message.
        private string mailBodyResetPassword(string email, string firstName, string link)
        {
            string body = string.Empty; // Initializes the body as an empty string.
            using (StreamReader reader = new StreamReader("./wwwroot/mailTemplateResetPassword.html")) // Opens the email template file.
            {
                body = reader.ReadToEnd(); // Reads the content of the file.
            }
            body = body.Replace("{username}", email); // Replaces the placeholder with the email.
            body = body.Replace("{firstName}", firstName); // Replaces the placeholder with the first name.
            body = body.Replace("{resetLink}", link); // Replaces the placeholder with the reset link.
            return body; // Returns the formatted email body.
        }
    }
}
