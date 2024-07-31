// Import necessary namespaces
using System; // Provides basic functionalities
using System.Collections.Generic; // Provides collection types like IEnumerable
using System.Linq; // Provides LINQ functionalities for data querying
using System.Threading.Tasks; // Provides asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Interface defining the contract for company repository operations
    public interface ICompanyRepository
    {
        // Method to get a company by its ID
        Company GetCompany(int? Id);

        // Method to get all companies
        IEnumerable<Company> GetAllCompanies();

        // Method to add a new company
        Company Add(Company company);

        // Method to update an existing company's details
        Company Update(Company companyChanges);

        // Method to delete a company by its ID
        Company Delete(int id);
    }
}
