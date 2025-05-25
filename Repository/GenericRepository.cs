using Kalanon_University.Data;
using Microsoft.EntityFrameworkCore;
using kalamon_University.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace kalamon_University.Repository
{
    public class GenericRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id) => await _dbSet.FindAsync(id);
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
        public virtual async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);
        public virtual void Update(TEntity entity) => _dbSet.Update(entity);
        public virtual void Delete(TEntity entity) => _dbSet.Remove(entity);
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }