// In: kalamon_University/Interfaces/IRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;

namespace kalamon_University.Interfaces
{
    // TEntity: هو نوع الشيء الذي سنتعامل معه (مثل Course أو Student)
    // TKey: هو نوع المفتاح الخاص به (مثل int أو Guid)
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task SaveChangesAsync();
    }
}