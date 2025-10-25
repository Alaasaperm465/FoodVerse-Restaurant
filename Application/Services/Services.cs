using Application.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class Services<T> : IServices<T> where T : class
    {
        private readonly IRepository<T> _repo;

        public Services(IRepository<T> repo) 
        {
            _repo = repo;
        }

        public async ValueTask<T> CreateAsync(T entity)
        {
            if (entity == null)
                return null;

            var nameProp = typeof(T).GetProperty("Name");
            var newName = nameProp.GetValue(entity)?.ToString()?.Trim().ToLower();
            var entities = await _repo.GetAllAsync();
            foreach (var item in entities)
            {
                var existingName = nameProp.GetValue(item)?.ToString()?.Trim().ToLower();
                if (existingName == newName)
                {
                    return null;
                }
            }

            return await _repo.AddAsync(entity);
        }

        public virtual async ValueTask<List<T>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async ValueTask<T> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async ValueTask UpdateAsync(T entity)
        {
            await _repo.UpdateAsync(entity);
        }

        public async ValueTask DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
