namespace kalamon_University.Interfaces;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    void Update(TEntity entity); // EF Core tracks changes, so often just modification is enough
    void Delete(TEntity entity);
    Task<int> SaveChangesAsync(); // Unit of Work
}