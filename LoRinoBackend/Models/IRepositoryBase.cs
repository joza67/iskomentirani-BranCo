// Import necessary namespaces
using System.Linq.Expressions; // Provides classes for representing expressions
using System.Linq; // Provides LINQ (Language Integrated Query) functionality
using System; // Provides fundamental classes and types

namespace LoRinoBackend.Models
{
    // Generic interface for repository operations
    public interface IRepositoryBase<T>
    {
        // Method to retrieve all entities of type T
        IQueryable<T> FindAll();

        // Method to retrieve entities of type T that match a specified condition
        // The condition is defined by an expression (lambda function)
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

        // Method to create a new entity of type T
        void Create(T entity);

        // Method to update an existing entity of type T
        void Update(T entity);

        // Method to delete an entity of type T
        void Delete(T entity);
    }
}
