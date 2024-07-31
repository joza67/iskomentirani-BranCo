// Importing necessary namespaces for functionalities related to LINQ and expressions
using System; // Provides fundamental types and functions
using System.Linq; // Provides LINQ functionality for querying collections
using System.Linq.Expressions; // Provides support for LINQ queries using expression trees

namespace LoRinoBackend.Repository
{
    // Abstract class providing a base implementation for a repository pattern
    // Generic parameter T ensures this class can be used with any entity type
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        // Protected property to access the database context
        protected AppDbContext RepositoryContext { get; set; }

        // Constructor to initialize the repository context
        // Dependency Injection can be used to provide the AppDbContext
        public RepositoryBase(AppDbContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        // Method to retrieve all entities of type T from the database
        // AsNoTracking() indicates that entities are not being tracked for changes
        public IQueryable<T> FindAll()
        {
            return this.RepositoryContext.Set<T>()
                .AsNoTracking(); // Improves performance for read-only operations
        }

        // Method to retrieve entities of type T based on a specific condition
        // Expression<Func<T, bool>> allows for the use of lambda expressions to specify the condition
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>()
                .Where(expression) // Filters entities based on the given condition
                .AsNoTracking(); // Improves performance for read-only operations
        }

        // Method to add a new entity to the database
        // The entity is added to the context, and changes are saved
        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity); // Adds the new entity to the set
            this.RepositoryContext.SaveChanges(); // Persists changes to the database
        }

        // Method to update an existing entity in the database
        // The entity is updated in the context, and changes are saved
        public void Update(T entity)
        {
            this.RepositoryContext.Set<T>().Update(entity); // Updates the existing entity
            this.RepositoryContext.SaveChanges(); // Persists changes to the database
        }

        // Method to delete an entity from the database
        // The entity is removed from the context, and changes are saved
        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity); // Removes the entity from the set
            this.RepositoryContext.SaveChanges(); // Persists changes to the database
        }
    }
}
