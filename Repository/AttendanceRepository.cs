// In: kalamon_University/Repository/AttendanceRepository.cs

using kalamon_University.Data;
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace kalamon_University.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
        }

        public async Task<bool> HasAttendanceRecordAsync(Guid studentId, int courseId, DateTime sessionDate)
        {
            // تحقق إذا كان هناك سجل لنفس الطالب في نفس المقرر في نفس اليوم
            return await _context.Attendances
                .AnyAsync(a => a.StudentId == studentId &&
                               a.CourseId == courseId &&
                               a.SessionDate.Date == sessionDate.Date);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        // In: Repository/AttendanceRepository.cs
        public async Task<int> GetAbsenceCountForStudentAsync(Guid studentId, int courseId)
        {
            return await _context.Attendances
                .CountAsync(a => a.StudentId == studentId &&
                                 a.CourseId == courseId &&
                                 a.IsPresent == false); // نعد فقط السجلات التي يكون فيها الطالب غائباً
        }
        public async Task<IEnumerable<Attendance>> GetAttendanceForStudentInCourseAsync(Guid studentId, int courseId)
        {
            return await _context.Attendances
                                 .Where(a => a.StudentId == studentId && a.CourseId == courseId)
                                 .OrderByDescending(a => a.SessionDate) // ترتيبها حسب التاريخ
                                 .ToListAsync();
        }
    }
}