using kalamon_University.Models.Entities;

namespace kalamon_University.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();
        Task<Student?> GetByUserIdAsync(Guid userId);
    }
}
