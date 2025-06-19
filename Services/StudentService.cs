using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using kalamon_University.Data;
using kalamon_University.Interfaces;
using kalamon_University.DTOs.Course;
using kalamon_University.DTOs.Common;
using kalamon_University.Models.Entities;

    namespace kalamon_University.Services
{
    /// <summary>
    /// فئة الخدمة التي تنفذ الواجهة IStudentService وتوفر منطق العمل لإدارة الطلاب.
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(AppDbContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Student-Specific Operations Implementation

        /// <inheritdoc />
        public async Task<IEnumerable<CourseDetailDto>> GetAllAvailableCoursesAsync()
        {
            try
            {
                // استخدم .Select لإنشاء كائن DTO جديد مباشرة من الاستعلام
                return await _context.Courses
                                     .Include(c => c.Professor.User) // نحتاج للـ Include للوصول للاسم
                                     .Select(course => new CourseDetailDto
                                     {
                                         Id = course.Id,
                                         Name = course.Name,
                                         // هنا نكسر الحلقة: نأخذ الاسم فقط
                                         ProfessorName = course.Professor != null ? course.Professor.User.FullName : "N/A",
                                         TheoreticalHours = course.TheoreticalHours,
                                         PracticalHours = course.PracticalHours
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب قائمة جميع الكورسات المتاحة.");
                throw; // أو تعامل مع الخطأ بشكل أفضل
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResult> EnrollInCourseAsync(Guid studentId, int courseId)
        {
            try
            {
                // التحقق من وجود الطالب والكورس (خطوة مهمة جداً)
                var studentExists = await _context.Students.AnyAsync(s => s.UserId == studentId);
                if (!studentExists)
                {
                    return ServiceResult.Failed("Student not found.");
                }

                var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
                if (!courseExists)
                {
                    return ServiceResult.Failed("Course not found.");
                }

                // التحقق مما إذا كان الطالب مسجلاً بالفعل
                var isAlreadyEnrolled = await _context.Enrollments
                    .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

                if (isAlreadyEnrolled)
                {
                    _logger.LogWarning("محاولة تسجيل فاشلة: الطالب {StudentId} مسجل بالفعل في الكورس {CourseId}.", studentId, courseId);
                    // أرجع رسالة خطأ واضحة
                    return ServiceResult.Failed("You are already enrolled in this course.");
                }

                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollmentDate = DateTime.UtcNow
                };

                await _context.Enrollments.AddAsync(enrollment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم تسجيل الطالب {StudentId} بنجاح في الكورس {CourseId}.", studentId, courseId);
                // أرجع نتيجة نجاح
                return ServiceResult.Succeeded("Successfully enrolled in the course.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء محاولة تسجيل الطالب {StudentId} في الكورس {CourseId}.", studentId, courseId);
                // أرجع خطأ عام
                return ServiceResult.Failed("An unexpected error occurred during enrollment.");
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Course>> GetMyEnrolledCoursesAsync(Guid studentId)
        {
            try
            {
                // جلب الكورسات من خلال جدول الربط Enrollments
                return await _context.Enrollments
               .Where(e => e.StudentId == studentId)
               .Include(e => e.Course)
               .ThenInclude(c => c.Professor)
               .ThenInclude(p => p.User)
               .Select(e => e.Course)
               .AsNoTracking()
               .ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب الكورسات المسجل بها الطالب {StudentId}.", studentId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Attendance>> GetMyAttendanceForCourseAsync(Guid studentId, int courseId)
        {
            try
            {
                // جلب سجلات الحضور لطالب معين في كورس معين
                return await _context.Attendances
                                     .Where(a => a.StudentId == studentId && a.CourseId == courseId)
                                     .OrderByDescending(a => a.SessionDate) // ترتيبها حسب التاريخ
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب سجل الحضور للطالب {StudentId} في الكورس {CourseId}.", studentId, courseId);
                throw;
            }
        }

        #endregion

        
    }
}