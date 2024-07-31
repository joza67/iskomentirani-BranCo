// Import necessary namespaces
using Microsoft.AspNetCore.Mvc.Rendering; // Provides classes and interfaces for rendering HTML elements in Razor views
using System.Collections.Generic; // Provides collection types like IEnumerable and List
using System.ComponentModel.Design; // Provides classes that manage design-time behavior

namespace LoRinoBackend.Models
{
    // Interface defining the contract for Movee event tag repository operations
    public interface IMoveeEventTagRepository
    {
        // Method to get all Movee tags
        IEnumerable<MoveeTag> GetMoveeTags();

        // Method to get a Movee tag by its ID
        MoveeTag GetMoveeTagById(int id);

        // Method to create a new Movee tag
        void CreateMoveeTag(MoveeTag moveeTag);

        // Method to delete a Movee tag
        MoveeTag DeleteMoveeTag(MoveeTag moveeTag);

        // Method to update an existing Movee tag
        MoveeTag UpdateMoveeTag(MoveeTag moveeTag);

        // Method to get all Movee event tags
        IEnumerable<MoveeEventTag> GetAllMoveeEventTags();

        // Method to get the company name associated with a specific tag ID
        string GetCompanyNameFromTagId(int id);

        // Method to get a Movee event tag by its ID
        MoveeEventTag GetMoveeEventTagById(int id);

        // Method to update a Movee event tag
        void UpdateMoveeEventTag(MoveeEventTag moveeEventTag);

        // Method to get tags for a dropdown list based on specific IDs and a company ID
        List<SelectListItem> TagsForDropDownList(List<int> ints, int companyId);

        // Method to get all tags for a dropdown list based on a company ID
        List<SelectListItem> AllTagsForDropDownList(int companyId);

        // Method to get a list of all Movee tags
        List<MoveeTag> TagsList();

        // Method to get all active Movee tags
        IEnumerable<MoveeTag> GetActiveMoveeTags();

        // Method to create a new Movee event tag
        void CreateMoveeEventTag(MoveeEventTag moveeEventTag, string by, long time);

        // Method to remove a Movee event tag
        MoveeEventTag RemoveMoveeEventTag(MoveeEventTag moveeEventTag, string by, long time);

        // Method to log the addition of a new event tag
        void LogAddedEventTag(int eventId, int eventTagId, string by, long time);

        // Method to log the removal of an event tag
        void LogRemovedEventTag(int eventId, int eventTagId, string by, long time);

        // Method to get logs of deleted tags for a specific user
        List<int> GetMyDeletedTagLogs(int eventId, string userId);

        // Method to get logs of added tags for a specific user
        List<int> GetMyAddedTagLogs(int eventId, string userId);

        // Method to get the owner of a specific Movee event tag
        string GetTagOwner(int moveeEventTagId);
    }
}
