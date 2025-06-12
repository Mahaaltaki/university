using kalamon_University.Models.Entities;
namespace kalamon_University.Interfaces
{
    public interface IStudentService
    {
        Task AddAsync(Student student);
        Task SaveChangesAsync();
    }
}