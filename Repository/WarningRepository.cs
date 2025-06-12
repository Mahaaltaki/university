// In: kalamon_University/Repository/WarningRepository.cs

using kalamon_University.Data;
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using kalamon_University.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kalamon_University.Repository
{
    public class WarningRepository : IWarningRepository
    {
        private readonly AppDbContext _context;

        public WarningRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Warning warning)
        {
            await _context.Warnings.AddAsync(warning);
        }

        public async Task<IEnumerable<Warning>> GetWarningsForStudentAsync(Guid studentId)
        {
            return await _context.Warnings
                .Include(w => w.Course) // لجلب اسم الكورس
                .Include(w => w.IssuedByUser) // لجلب اسم من أصدر الإنذار
                .OrderByDescending(w => w.DateIssued) // ترتيب الإنذارات من الأحدث للأقدم
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        // In: Repository/WarningRepository.cs
        public async Task<bool> HasAbsenceWarningForCourseAsync(Guid studentId, int courseId)
        {
            // نفترض أن لديك Enum اسمه WarningType
            return await _context.Warnings
                .AnyAsync(w => w.StudentId == studentId &&
                               w.CourseId == courseId &&
                               w.Type == WarningType.Absence); // تحقق من وجود إنذار من نوع "غياب"
        }
    }
}