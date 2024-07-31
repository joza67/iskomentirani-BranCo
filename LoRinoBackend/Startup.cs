using LoRinoBackend.Hubs; // Imports the Hubs namespace for SignalR hub communication.
using LoRinoBackend.Security; // Imports the Security namespace for custom security implementations.
using Microsoft.AspNetCore.Builder; // Provides methods to configure the application's request pipeline.
using Microsoft.AspNetCore.Hosting; // Provides types for hosting web applications.
using Microsoft.AspNetCore.Http; // Provides HTTP context and related services.
using Microsoft.AspNetCore.Identity; // Provides types for Identity management.
using Microsoft.AspNetCore.Localization; // Provides localization features for the application.
using Microsoft.Extensions.Configuration; // Provides configuration settings.
using Microsoft.Extensions.DependencyInjection; // Provides methods for service registration.
using Microsoft.Extensions.Hosting; // Provides hosting environment information.
using Microsoft.Extensions.Logging; // Provides logging services.
using Microsoft.Extensions.Options; // Provides options pattern for configurations.
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Provides MySQL-specific Entity Framework Core extensions.
using System; // Provides basic types and functionality.
using System.Collections.Generic; // Provides collection types.
using System.Globalization; // Provides globalization and localization support.
using System.Text.Json.Serialization; // Provides JSON serialization options.
using Microsoft.AspNetCore.Authorization; // Provides authorization features.

namespace LoRinoBackend
{
    public class Startup
    {
        private IConfiguration _config; // Stores the configuration settings.
        private IWebHostEnvironment _env; // Stores the hosting environment information.

        // Constructor to initialize configuration and environment.
        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        // Method to configure services for the application.
        public void ConfigureServices(IServiceCollection services)
        {
            // Builds the connection string based on the environment.
            var connectionString = _env.IsDevelopment()
                ? _config.GetConnectionString("DevServer")
                : $"server={Environment.GetEnvironmentVariable("DB_HOST")};database={Environment.GetEnvironmentVariable("DB_NAME")};port={Environment.GetEnvironmentVariable("DB_PORT")};User ID={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

            // Configures the DbContext with MySQL.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)) // Configures MySQL with automatic server version detection.
                       .EnableDetailedErrors() // Enables detailed error messages for debugging.
                       .EnableSensitiveDataLogging(); // Logs sensitive data for debugging.
            });

            // Adds logging services.
            services.AddLogging(builder =>
            {
                builder.AddConsole(); // Logs to the console.
                builder.AddDebug(); // Logs to the debug output.
            });

            // Configures Identity services for user authentication and management.
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true; // Requires confirmed email for signing in.
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation"; // Sets a custom email confirmation token provider.
                options.Tokens.PasswordResetTokenProvider = "CustomEmailConfirmation"; // Sets a custom password reset token provider.
                options.Lockout.MaxFailedAccessAttempts = 5; // Sets the maximum failed access attempts before lockout.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Sets the default lockout time span.
            })
            .AddEntityFrameworkStores<AppDbContext>() // Configures Entity Framework stores.
            .AddDefaultTokenProviders() // Adds default token providers.
            .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation"); // Adds a custom email confirmation token provider.

            // Configures email settings using the SMTP configuration section.
            services.Configure<SmtpSettings>(_config.GetSection("Smtp"));
            services.AddTransient<IEmailSender, SmtpEmailSender>(); // Registers the SMTP email sender service.

            // Configures token provider options.
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(8)); // Sets the token lifespan for data protection tokens.
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3)); // Sets the token lifespan for custom email confirmation tokens.

            // Configures password options.
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6; // Sets the required password length.
                options.Password.RequiredUniqueChars = 3; // Sets the number of required unique characters.
                options.Password.RequireNonAlphanumeric = false; // Does not require non-alphanumeric characters.
                options.Password.RequireUppercase = false; // Does not require uppercase characters.
            });

            // Configures application cookie settings.
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied"); // Sets the path for access denied.
            });

            // Configures authorization policies.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") &&
                    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
                    context.User.HasClaim(claim => claim.Type == "Delete Role" && claim.Value == "true") ||
                    context.User.IsInRole("Super Admin")
                )); // Adds a policy for deleting roles.

                options.AddPolicy("EditRolePolicy", policy =>
                     policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement())); // Adds a policy for editing roles.

                options.AddPolicy("AdminRolePolicy", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || context.User.IsInRole("Super Admin")
                )); // Adds a policy for admin roles.
                options.AddPolicy("SuperAdmin", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole("Super Admin")
                )); // Adds a policy for super admin roles.
            });

            // Registers custom authorization handlers.
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>(); // Registers the handler for editing admin roles and claims.
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>(); // Registers the handler for super admin roles.

            // Configures CORS (Cross-Origin Resource Sharing).
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://localhost",
                                "http://10.0.5.5",
                                "http://10.0.5.5:5051",
                                "https://10.0.5.5:5050",
                                "http://local.microlink.hr",
                                "http://127.0.0.1:5500")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()); // Allows credentials and specific origins.
            });

            // Configures JSON serialization options.
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Ignores reference cycles in JSON serialization.
                });

            // Registers MVC services.
            services.AddMvc();

            // Registers scoped services for various repositories.
            services.AddScoped<IAlarmSoundRepository, SQLAlarmSoundRepository>(); // Registers the alarm sound repository.
            services.AddScoped<ICompanyRepository, SQLCompanyRepository>(); // Registers the company repository.
            services.AddScoped<IDecodedDataRepository, SQLDecodedDataRepository>(); // Registers the decoded data repository.
            services.AddScoped<IDeviceRepository, SQLDeviceRepository>(); // Registers the device repository.
            services.AddScoped<IDeviceTypeRepository, SQLDeviceTypeRepository>(); // Registers the device type repository.
            services.AddScoped<IEmailSender, SmtpEmailSender>(); // Registers the email sender service.
            services.AddScoped<ILocationRepository, SQLLocationRepository>(); // Registers the location repository.
            services.AddScoped<ILoRaDataRepository, SQLLoRaRepository>(); // Registers the LoRa data repository.
            services.AddScoped<IMoveeDataRepository, SQLMoveeDataRepository>(); // Registers the movee data repository.
            services.AddScoped<IMoveeEventCommentRepository, SQLMoveeEventCommentRepository>(); // Registers the movee event comment repository.
            services.AddScoped<IMoveeEventRepository, SQLMoveeEventRepository>(); // Registers the movee event repository.
            services.AddScoped<IMoveeEventTagRepository, SQLMoveeEventTagRepository>(); // Registers the movee event tag repository.

            services.AddSignalR(); // Registers SignalR services for real-time communication.

            services.AddScoped<MailTimer>(); // Registers the MailTimer service.
        }

        // Method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Uses the developer exception page in development.
            }
            else
            {
                app.UseExceptionHandler("/Error"); // Uses the error handler page in production.
                app.UseStatusCodePagesWithReExecute("/Error/{0}"); // Re-executes the request for status code pages.
            }

            app.UseStaticFiles(); // Serves static files.

            // Configures request localization.
            var defaultCulture = "hr-Hr"; // Sets the default culture.
            var ci = new CultureInfo(defaultCulture); // Creates a CultureInfo object for the default culture.
            ci.NumberFormat.NumberDecimalSeparator = "."; // Sets the decimal separator.
            ci.NumberFormat.CurrencyDecimalSeparator = "."; // Sets the currency decimal separator.

            app.UseRequestLocalization(new RequestLocalizationOptions()
            {
                DefaultRequestCulture = new RequestCulture(ci), // Sets the default request culture.
                SupportedCultures = new List<CultureInfo>() { ci }, // Sets the supported cultures.
                SupportedUICultures = new List<CultureInfo>() { ci } // Sets the supported UI cultures.
            });

            app.UseCors("CorsPolicy"); // Applies the CORS policy.

            app.UseRouting(); // Adds routing middleware.

            app.UseAuthentication(); // Adds authentication middleware.
            app.UseAuthorization(); // Adds authorization middleware.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<WanesyHub>("/wanesyhub"); // Maps the SignalR hub endpoint.
                endpoints.MapRazorPages(); // Maps Razor Pages.
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=MV}/{action=Index}/{id?}/{s?}"); // Maps the default route.
            });
        }
    }
}
