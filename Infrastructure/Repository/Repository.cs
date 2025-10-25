using Application.Context;
using Application.Contract;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly RestaurantContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(RestaurantContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async ValueTask<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async ValueTask<List<TEntity>> GetAllAsync()
        {
            var prop = typeof(TEntity).GetProperty("IsDeleted");
            if (prop != null)
            {
                return await _dbSet
                    .Where(e => !EF.Property<bool>(e, "IsDeleted"))
                    .ToListAsync();
            }
            return await _dbSet.ToListAsync();
        }

        public virtual async ValueTask<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async ValueTask UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async ValueTask DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return;

            var prop = entity.GetType().GetProperty("IsDeleted");
            if (prop != null)
            {
                // soft delete
                prop.SetValue(entity, true);
                _dbSet.Update(entity);
            }
            else
            {
                // hard delete
                _dbSet.Remove(entity);
            }

            await _context.SaveChangesAsync();
        }
    }
}
