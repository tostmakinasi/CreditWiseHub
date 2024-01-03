using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Commons;
using CreditWiseHub.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CreditWiseHub.Repository.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class, IEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.AsNoTracking().ToListAsync();

        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> items)
        {
            await _dbSet.AddRangeAsync(items);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);

        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> items)
        {
            _dbSet.RemoveRange(items);
        }
    }
}
