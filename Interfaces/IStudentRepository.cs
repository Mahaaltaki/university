using kalamon_University.Models.Entities;

namespace kalamon_University.Interfaces
{
    public interface IStudentRepository
    {
        Task AddAsync(Student student);
        Task SaveChangesAsync();
        void Delete(Student student);
        Task<Student> GetByUserIdAsync(Guid userId);
    }
}
