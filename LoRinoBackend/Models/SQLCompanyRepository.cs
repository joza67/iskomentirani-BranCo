using Microsoft.AspNetCore.Mvc; // For MVC components (not used in the current code but commonly used in controllers)
using Microsoft.AspNetCore.Http; // For HTTP context (not used in the current code but useful in controllers)
using Microsoft.AspNetCore.Identity; // For managing application users and roles
using Microsoft.EntityFrameworkCore; // For Entity Framework Core functionalities
using System; // For general .NET functionalities
using System.Collections.Generic; // For collections like IEnumerable<T>
using System.Linq; // For LINQ operations such as ToList and Find
using System.Threading.Tasks; // For asynchronous programming (not used in the current code)
using System.Security.Claims; // For working with claims-based identity (not used in the current code)

namespace LoRinoBackend.Models
{
    // Repository class for managing company data in the SQL database
    public class SQLCompanyRepository : ICompanyRepository
    {
        // Private fields for database context and user manager
        private readonly AppDbContext contex;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to initialize the repository with the necessary services
        public SQLCompanyRepository(AppDbContext contex, UserManager<ApplicationUser> userManager)
        {
            this.contex = contex;
            _userManager = userManager;
        }

        // Method to add a new company to the database
        public Company Add(Company company)
        {
            // Add the company entity to the context
            contex.Company.Add(company);
            // Save changes to the database
            contex.SaveChanges();
            return company;
        }

        // Method to delete a company by its ID
        public Company Delete(int id)
        {
            // Find the company by its ID
            Company company = contex.Company.Find(id);
            if (company != null)
            {
                // Remove the company entity from the context
                contex.Company.Remove(company);
                // Save changes to the database
                contex.SaveChanges();
            }
            return company;
        }

        // Method to retrieve all companies from the database
        public IEnumerable<Company> GetAllCompanies()
        {
            // Fetch and return all companies as a list
            return contex.Company.ToList();
        }

        // Method to retrieve a company by its ID
        public Company GetCompany(int? Id)
        {
            // Find and return the company by its ID
            return contex.Company.Find(Id);
        }

        // Method to update an existing company's details
        public Company Update(Company companyChanges)
        {
            // Attach the company entity to the context
            var company = contex.Company.Attach(companyChanges);
            // Mark the entity as modified
            company.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            // Save changes to the database
            contex.SaveChanges();
            return companyChanges;
        }
    }
}
