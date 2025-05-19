using kalamon_University.Models.Entities;
using Kalanon_University.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using kalamon_University.Interfaces;

public class UserRepository : IRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<User> _dbSet;

    public UserRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<User>();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(User entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(User entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(User entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}
