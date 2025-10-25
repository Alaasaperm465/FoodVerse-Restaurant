namespace Application.Contract
{
    public interface IRepository<TEntity> where TEntity : class
    {
        ValueTask<TEntity> AddAsync(TEntity entity);
        ValueTask UpdateAsync(TEntity entity);
        ValueTask<TEntity> GetByIdAsync(int id);
        ValueTask<List<TEntity>> GetAllAsync();
        ValueTask DeleteAsync(int id);
    }
}
