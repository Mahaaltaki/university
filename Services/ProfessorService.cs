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
using ClosedXML.Excel;
using kalamon_University.DTOs.Attendance;

namespace kalamon_University.Services
{
    /// <summary>
    /// فئة الخدمة التي تنفذ الواجهة IProfessorService وتوفر منطق العمل لإدارة الأساتذة.
    /// </summary>
    public class ProfessorService : IProfessorService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProfessorService> _logger;
        private readonly IFileStorageService _fileStorage; 
        private readonly INotificationService _notificationService;

        /// <summary>
        /// المنشئ (Constructor) الذي يقوم بحقن التبعيات المطلوبة.
        /// </summary>
        /// <param name="context">سياق قاعدة البيانات للتفاعل معها.</param>
        /// <param name="logger">خدمة لتسجيل المعلومات والأخطاء.</param>
        public ProfessorService(
        AppDbContext context,
        ILogger<ProfessorService> logger,
        IFileStorageService fileStorage,
        INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _fileStorage = fileStorage;
            _notificationService = notificationService;
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

        public async Task<ServiceResult<IEnumerable<DTOs.Attendance.AttendanceRecordDto>>> GetAttendanceForCourseAsync(Guid professorId, int courseId)
        {
            var isHisCourse = await _context.Courses.AnyAsync(c => c.Id == courseId && c.ProfessorId == professorId);
            if (!isHisCourse)
            {
                return ServiceResult<IEnumerable<DTOs.Attendance.AttendanceRecordDto>>.Failed("Course not found or you are not assigned to it.");
            }

            var attendanceRecords = await _context.Attendances
                .Where(a => a.CourseId == courseId)
                .Include(a => a.Student.User)
                .OrderBy(a => a.Student.User.FullName)
                .ThenByDescending(a => a.SessionDate)
                .Select(a => new DTOs.Attendance.AttendanceRecordDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentName = a.Student.User.FullName,
                    SessionDate = a.SessionDate,
                    IsPresent = a.IsPresent,
                    Notes = a.Notes
                })
                .AsNoTracking()
                .ToListAsync();

            return ServiceResult<IEnumerable<DTOs.Attendance.AttendanceRecordDto>>.Succeeded(attendanceRecords);
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

        #region Attendance Management

        public async Task<ServiceResult<DTOs.Attendance.AttendanceRecordDto>> AddAttendanceAsync(Guid professorId, int courseId, CreateAttendanceDto dto)
        {
            // التحقق من أن الكورس يخص الأستاذ
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.ProfessorId == professorId);
            if (course == null)
            {
                return ServiceResult<DTOs.Attendance.AttendanceRecordDto>.Failed("Course not found or you are not assigned to it.");
            }

            // التحقق من أن الطالب مسجل في هذا الكورس
            var isEnrolled = await _context.Enrollments.AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == courseId);
            if (!isEnrolled)
            {
                return ServiceResult<DTOs.Attendance.AttendanceRecordDto>.Failed("This student is not enrolled in this course.");
            }

            // التحقق من عدم وجود سجل مكرر لنفس الطالب في نفس اليوم
            var alreadyExists = await _context.Attendances.AnyAsync(a => a.StudentId == dto.StudentId && a.CourseId == courseId && a.SessionDate.Date == dto.SessionDate.Date);
            if (alreadyExists)
            {
                return ServiceResult<DTOs.Attendance.AttendanceRecordDto>.Failed("Attendance for this student has already been recorded for this session date.");
            }

            var newAttendance = new Attendance
            {
                StudentId = dto.StudentId,
                CourseId = courseId,
                SessionDate = dto.SessionDate,
                IsPresent = dto.IsPresent,
                Notes = dto.Notes
            };

            await _context.Attendances.AddAsync(newAttendance);
            await _context.SaveChangesAsync();

            // يمكنك هنا استدعاء دالة التحقق من الإنذار إذا أردت
            // await _attendanceService.CheckAndIssueAbsenceWarningAsync(dto.StudentId, courseId);

            var resultDto = new DTOs.Attendance.AttendanceRecordDto
            {
                
                Id = newAttendance.Id,
                StudentId = newAttendance.StudentId,
                StudentName = (await _context.Users.FindAsync(dto.StudentId))?.FullName ?? "N/A",
                SessionDate = newAttendance.SessionDate,
                IsPresent = newAttendance.IsPresent,
                Notes = newAttendance.Notes
            };

            return ServiceResult<DTOs.Attendance.AttendanceRecordDto>.Succeeded(resultDto);
        }

        public async Task<ServiceResult> UpdateAttendanceAsync(Guid professorId, int courseId, int attendanceId, UpdateAttendanceDto dto)
        {
            // التحقق من أن سجل الحضور يتبع لكورس يدرسه الأستاذ
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.Id == attendanceId && a.Course.ProfessorId == professorId && a.CourseId == courseId);

            if (attendance == null)
            {
                return ServiceResult.Failed("Attendance record not found or you do not have permission to edit it.");
            }

            attendance.IsPresent = dto.IsPresent;
            attendance.Notes = dto.Notes;
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();

            return ServiceResult.Succeeded("Attendance record updated successfully.");
        }

        public async Task<ServiceResult> DeleteAttendanceAsync(Guid professorId, int courseId, int attendanceId)
        {
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.Id == attendanceId && a.Course.ProfessorId == professorId && a.CourseId == courseId);

            if (attendance == null)
            {
                return ServiceResult.Failed("Attendance record not found or you do not have permission to delete it.");
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return ServiceResult.Succeeded("Attendance record deleted successfully.");
        }

        #endregion

        #region Excel Export

        // الدالة القديمة ExportAttendanceToExcelAsync يمكن حذفها أو جعلها private

        public async Task<ServiceResult> ExportAndNotifyAdminsAsync(Guid professorId, int courseId)
        {
            // 1. إنشاء ملف الإكسل (نفس منطق الإنشاء من الإجابة السابقة)
            var excelResult = await ExportAttendanceToExcelAsync(professorId, courseId); // استدعاء الدالة الخاصة
            if (!excelResult.Success)
            {
                return ServiceResult.Failed(excelResult.Errors);
            }

            // 2. حفظ الملف باستخدام خدمة التخزين
            var (fileContents, fileName) = excelResult.Data;
            var fileUrl = await _fileStorage.SaveFileAsync(fileContents, fileName);

            // 3. جلب كل مسؤولي النظام
            var adminIds = await _context.Users
                .Where(u => u.Role == kalamon_University.Models.Enums.Role.Admin)
                .Select(u => u.Id)
                .ToListAsync();

            if (!adminIds.Any())
            {
                _logger.LogWarning("No admins found to send attendance report notification.");
                return ServiceResult.Succeeded("Report generated, but no admins found to notify.");
            }

            // 4. إرسال إشعار لكل مسؤول
            var professor = await _context.Users.FindAsync(professorId);
            var course = await _context.Courses.FindAsync(courseId);

            var message = $"Professor '{professor?.FullName}' has generated the attendance report for course '{course?.Name}'.";

            foreach (var adminId in adminIds)
            {
                await _notificationService.SendNotificationAsync(new CreateNotificationDto
                {
                    TargetUserId = adminId,
                    Message = message,
                    RelatedEntityType = "FileLink", // نوع جديد للإشارة إلى أنه رابط
                                                    // سنضع اسم الملف في RelatedEntityName ورابطه في Message أو يمكن تخصيص حقل له
                    RelatedEntityName = fileName
                });
            }

            return ServiceResult.Succeeded("Attendance report has been generated and admins have been notified.");
        }

        //  هذه الدالة private الآن لأنها مساعدة
        private async Task<ServiceResult<(byte[] fileContents, string fileName)>> ExportAttendanceToExcelAsync(Guid professorId, int courseId)
        {
           
            var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == courseId && c.ProfessorId == professorId);
            if (course == null)
            {
                return ServiceResult<(byte[], string)>.Failed("Course not found or you are not assigned to it.");
            }

            var attendanceData = await GetAttendanceForCourseAsync(professorId, courseId);
            if (!attendanceData.Success || !attendanceData.Data.Any())
            {
                return ServiceResult<(byte[], string)>.Failed("No attendance data available to export.");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance");
                worksheet.Cell("A1").Value = "Student Name";
                worksheet.Cell("B1").Value = "Student ID";
                worksheet.Cell("C1").Value = "Session Date";
                worksheet.Cell("D1").Value = "Status";
                worksheet.Cell("E1").Value = "Notes";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int currentRow = 2;
                foreach (var record in attendanceData.Data)
                {
                    worksheet.Cell(currentRow, 1).Value = record.StudentName;
                    worksheet.Cell(currentRow, 2).Value = record.StudentId.ToString();
                    worksheet.Cell(currentRow, 3).Value = record.SessionDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 4).Value = record.IsPresent ? "Present" : "Absent";
                    worksheet.Cell(currentRow, 5).Value = record.Notes;
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var fileName = $"Attendance_{course.Name.Replace(" ", "_")}_{Guid.NewGuid().ToString().Substring(0, 8)}.xlsx";
                    return ServiceResult<(byte[], string)>.Succeeded((content, fileName));
                }
            }
        }
    
    #endregion

}
}