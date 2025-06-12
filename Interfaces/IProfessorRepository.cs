using kalamon_University.Models.Entities;

namespace kalamon_University.Interfaces
{
    public interface IProfessorRepository
    {
        Task AddAsync(Professor professor);
        Task SaveChangesAsync();
        Task<Professor?> GetByUserIdAsync(Guid userId);
        void Delete(Professor professor);

    }
}
