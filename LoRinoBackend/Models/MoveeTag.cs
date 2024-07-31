// Importing necessary namespaces for .NET functionalities and data annotations
using System.Collections.Generic; // Provides generic collections like List
using System.ComponentModel; // Provides attributes for controlling the display of properties
using System.ComponentModel.DataAnnotations; // Provides attributes for data validation
using System.ComponentModel.DataAnnotations.Schema; // Provides attributes for mapping entities to the database schema

namespace LoRinoBackend.Models
{
    // Class representing a tag that can be associated with Movee events
    public class MoveeTag
    {
        // Unique identifier for the Movee tag
        public int Id { get; set; }

        // Display attribute to set a friendly name for the property in UI or metadata
        [Display(Name = "Naziv")] // "Naziv" means "Name" in Croatian
        public string Name { get; set; }

        // Display attribute to set a friendly name for the property in UI or metadata
        [Display(Name = "Aktivan")] // "Aktivan" means "Active" in Croatian
        public bool Active { get; set; }

        // Display attribute to set a friendly name for the property in UI or metadata
        [Display(Name = "Tvrtka")] // "Tvrtka" means "Company" in Croatian
        public int CompanyId { get; set; }

        // Foreign key attribute specifying the navigation property
        // This property represents a list of MoveeEventTag entities associated with this MoveeTag
        [ForeignKey("MoveeTagId")] // "MoveeTagId" refers to the foreign key in the MoveeEventTag table
        public List<MoveeEventTag> MoveeEventTag { get; set; }
    }
}
