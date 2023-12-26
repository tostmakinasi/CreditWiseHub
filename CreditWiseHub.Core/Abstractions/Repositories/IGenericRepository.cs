using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Abstractions.Repositories
{
    /// <summary>
    /// Generic repository interface used for general database operations.
    /// </summary>
    /// <typeparam name="T">Type of the database entity.</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Asynchronously retrieves an entity with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity with the specified ID.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all entities without any conditions.
        /// </summary>
        /// <returns>Query for all entities.</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Queries entities that meet the specified conditions.
        /// </summary>
        /// <param name="expression">Filtering conditions.</param>
        /// <returns>Query for entities that meet the specified conditions.</returns>
        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Checks if there is any entity that meets the specified conditions.
        /// </summary>
        /// <param name="expression">Filtering conditions.</param>
        /// <returns>"True" if there is any entity that meets the specified conditions, otherwise "false".</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Asynchronously adds the specified entities.
        /// </summary>
        /// <param name="items">Collection of entities to be added.</param>
        Task AddRangeAsync(IEnumerable<T> items);

        /// <summary>
        /// Asynchronously adds the specified entity.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        void Update(T entity);

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        void Remove(T entity);

        /// <summary>
        /// Removes the specified entities.
        /// </summary>
        /// <param name="items">Collection of entities to be removed.</param>
        void RemoveRange(IEnumerable<T> items);
    }
}
