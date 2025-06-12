// kalamon_University/Repository/CourseRepository.cs

using kalamon_University.Data;
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kalamon_University.Repository
{
    // يجب أن ينفذ الكلاس كل دوال الواجهة
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        // <-- إضافة: Constructor ضروري لعمل Dependency Injection
        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync(bool includeProfessorDetails = false)
        {
            IQueryable<Course> query = _context.Courses;

            if (includeProfessorDetails)
            {
                // الكود هنا صحيح وممتاز
                query = query.Include(c => c.Professor)
                             .ThenInclude(p => p.User);
            }

            return await query.ToListAsync();
        }

        // --- إضافة: الدوال الناقصة التي يجب تنفيذها ---

        public async Task<Course?> GetByIdAsync(int courseId)
        {
            return await _context.Courses.FindAsync(courseId);
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public void Update(Course course)
        {
            // EF Core يتتبع التغييرات، أحيانًا يكون هذا كافيًا إذا كان الكيان متتبعًا
            // الطريقة الأكثر أمانًا هي تحديد حالته بشكل صريح
            _context.Entry(course).State = EntityState.Modified;
        }

        public void Delete(Course course)
        {
            _context.Courses.Remove(course);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}