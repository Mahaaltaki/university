// kalamon_University/Interfaces/ICourseRepository.cs
using kalamon_University.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kalamon_University.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync(bool includeProfessorDetails = false);
        Task<Course?> GetByIdAsync(int courseId);
        Task AddAsync(Course course);
        void Update(Course course);
        void Delete(Course course);
        Task SaveChangesAsync();
    }
}