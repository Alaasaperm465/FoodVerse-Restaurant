using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IServices<T> where T : class
    {
        ValueTask<T> CreateAsync(T entity); 
        ValueTask<List<T>> GetAllAsync();
        ValueTask<T> GetByIdAsync(int id);
        ValueTask UpdateAsync(T entity);
        ValueTask DeleteAsync(int id);
    }
}
