// Import necessary namespaces
using System.Collections.Generic; // Provides collection types like List and IEnumerable

namespace LoRinoBackend.Models
{
    // Interface defining the contract for Movee event comment repository operations
    public interface IMoveeEventCommentRepository
    {
        // Method to get all comments associated with a specific event ID
        List<MoveeEventComment> GetAllCommentsByEventId(int eventId);

        // Method to add a new comment to an event
        MoveeEventComment AddComment(MoveeEventComment moveeEventComment);

        // Method to get details of a specific comment by its ID
        MoveeEventComment GetCommentDetails(int commentId);

        // Method to delete a comment by its ID
        MoveeEventComment DeleteComment(int commentId);

        // Method to update an existing comment
        MoveeEventComment Update(MoveeEventComment commentChanges);

        // Method to get logs related to a specific event ID
        List<LogViewModel> GetLogs(int eventId);

        // Method to get all event tag logs associated with a specific event ID
        IEnumerable<EventTagLog> GetAllEventTagsLogByEventId(int eventId);

        // Method to get all Movee event tags associated with a specific event ID
        IEnumerable<MoveeEventTag> GetAllMoveeEventTagsByEventId(int eventId);

        // Method to get all Movee tags
        IEnumerable<MoveeTag> GetMoveeTags();
    }
}
