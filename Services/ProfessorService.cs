using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using kalamon_University.Data; 
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Notification;
using kalamon_University.DTOs.ProfessorPortal;

namespace kalamon_University.Services
{
    /// <summary>
    /// فئة الخدمة التي تنفذ الواجهة IProfessorService وتوفر منطق العمل لإدارة الأساتذة.
    /// </summary>
    public class ProfessorService : IProfessorService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProfessorService> _logger;

        /// <summary>
        /// المنشئ (Constructor) الذي يقوم بحقن التبعيات المطلوبة.
        /// </summary>
        /// <param name="context">سياق قاعدة البيانات للتفاعل معها.</param>
        /// <param name="logger">خدمة لتسجيل المعلومات والأخطاء.</param>
        public ProfessorService(AppDbContext context, ILogger<ProfessorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Professor>> GetAllAsync()
        {
            try
            {
                // نستخدم Include لجلب بيانات المستخدم المرتبطة بكل أستاذ (Eager Loading).
                // من الأفضل عدم جلب TaughtCourses هنا لتجنب تحميل بيانات ضخمة في قائمة العرض العام.
                return await _context.Professors
                                     .Include(p => p.User)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب قائمة جميع الأساتذة.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Professor?> GetByIdAsync(Guid professorId)
        {
            try
            {
                // عند طلب أستاذ واحد، من المفيد جلب المواد التي يدرسها أيضًا.
                return await _context.Professors
                                     .Include(p => p.User)
                                     .Include(p => p.TaughtCourses)
                                     .FirstOrDefaultAsync(p => p.UserId == professorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء البحث عن أستاذ بالمعرف: {ProfessorId}", professorId);
                throw;
            }
        }
        #region New Professor-Specific Operations

        public async Task<IEnumerable<ProfessorCourseDto>> GetMyCoursesAsync(Guid professorId)
        {
            return await _context.Courses
                .Where(c => c.ProfessorId == professorId)
                .Select(c => new ProfessorCourseDto
                {
                    CourseId = c.Id,
                    CourseName = c.Name,
                    TotalHours = c.TotalHours
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ServiceResult<IEnumerable<EnrolledStudentDto>>> GetStudentsInCourseAsync(Guid professorId, int courseId)
        {
            // خطوة أمان هامة: التحقق من أن هذا الكورس يخص الأستاذ الحالي
            var isHisCourse = await _context.Courses.AnyAsync(c => c.Id == courseId && c.ProfessorId == professorId);
            if (!isHisCourse)
            {
                return ServiceResult<IEnumerable<EnrolledStudentDto>>.Failed("Course not found or you are not assigned to it.");
            }

            var students = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student.User) // للوصول لاسم الطالب
                .Select(e => new EnrolledStudentDto
                {
                    StudentId = e.StudentId,
                    FullName = e.Student.User.FullName,
                    Email = e.Student.User.Email
                })
                .AsNoTracking()
                .ToListAsync();

            return ServiceResult<IEnumerable<EnrolledStudentDto>>.Succeeded(students);
        }

        public async Task<ServiceResult<IEnumerable<AttendanceRecordDto>>> GetAttendanceForCourseAsync(Guid professorId, int courseId)
        {
            var isHisCourse = await _context.Courses.AnyAsync(c => c.Id == courseId && c.ProfessorId == professorId);
            if (!isHisCourse)
            {
                return ServiceResult<IEnumerable<AttendanceRecordDto>>.Failed("Course not found or you are not assigned to it.");
            }

            var attendanceRecords = await _context.Attendances
                .Where(a => a.CourseId == courseId)
                .Include(a => a.Student.User)
                .OrderBy(a => a.Student.User.FullName)
                .ThenByDescending(a => a.SessionDate)
                .Select(a => new AttendanceRecordDto
                {
                    AttendanceId = a.Id,
                    StudentId = a.StudentId,
                    StudentName = a.Student.User.FullName,
                    SessionDate = a.SessionDate,
                    IsPresent = a.IsPresent,
                    Notes = a.Notes
                })
                .AsNoTracking()
                .ToListAsync();

            return ServiceResult<IEnumerable<AttendanceRecordDto>>.Succeeded(attendanceRecords);
        }

        public async Task<ServiceResult> SendNotificationToCourseAsync(Guid professorId, int courseId, string message)
        {
            var isHisCourse = await _context.Courses.AnyAsync(c => c.Id == courseId && c.ProfessorId == professorId);
            if (!isHisCourse)
            {
                return ServiceResult.Failed("Course not found or you are not assigned to it.");
            }

            // جلب جميع الطلاب المسجلين في هذا الكورس
            var studentIds = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToListAsync();

            if (!studentIds.Any())
            {
                return ServiceResult.Failed("No students are enrolled in this course to notify.");
            }

            var notifications = studentIds.Select(studentId => new Notification
            {
                UserId = studentId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                RelatedEntityType = "Course",
                RelatedEntityId = courseId
            }).ToList();

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();

            return ServiceResult.Succeeded($"Notification sent to {notifications.Count} students.");
        }

        public async Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(Guid professorId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == professorId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                })
                .AsNoTracking()
                .ToListAsync();
        }

        #endregion

    }
}