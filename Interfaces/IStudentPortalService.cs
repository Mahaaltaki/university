// kalamon_University.Core/Interfaces/IStudentPortalService.cs
using kalamon_University.DTOs.Course; // For CourseDto
using kalamon_University.DTOs.Attendance; // For AttendanceRecordDto
using kalamon_University.DTOs.Common; // For ServiceResult
using kalamon_University.DTOs.StudentPortal; // For Student specific DTOs

// --- DTOs that might be specific to Student Portal or reused ---
// (Define these in kalamon_University/DTOs/StudentPortal/ or relevant folders)
// namespace kalamon_University.DTOs.StudentPortal;
// public record StudentProfileDto(Guid StudentId, string Name, string Email /*, other student specific info */);
// public record UpdateStudentProfileDto(string? Name /*, other updatable student specific info */);
// public record EnrolledCourseDto(int CourseId, string CourseName, string ProfessorName, int AbsenceCount, int MaxAbsenceLimit);
// public record CourseAttendanceDetailsDto(string CourseName, DateTime SessionDate, bool IsPresent);
// public record WarningDto(string CourseName, string WarningMessage, DateTime DateIssued);

namespace UniversityApi.Core.Interfaces.Services;

using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Course;
using kalamon_University.DTOs.StudentPortal;
using kalamon_University.DTOs.Warning; // If you have specific Warning DTOs
using kalamon_University.DTOs.Notification; // For NotificationDto


public interface IStudentPortalService
{
    // --- Profile Management ---
    Task<ServiceResult<StudentProfileDto>> GetStudentProfileAsync(Guid studentUserId); // studentUserId is User.Id
    Task<ServiceResult> UpdateStudentProfileAsync(Guid studentUserId, UpdateStudentProfileDto profileDto);

    // --- Course Enrollment and Viewing ---
    Task<ServiceResult<IEnumerable<CourseDto>>> GetAvailableCoursesAsync(Guid studentUserId, string? searchQuery = null); // Courses student can enroll in
    Task<ServiceResult<IEnumerable<EnrolledCourseDto>>> GetEnrolledCoursesAsync(Guid studentUserId);
    Task<ServiceResult<CourseDto>> GetCourseDetailsAsync(int courseId, Guid studentUserId); // Course details if enrolled or available

    // --- Course Registration (Enroll/Unenroll) ---
    // Note: Enrollment/Unenrollment might also be part of ICourseService if admin can do it.
    // If Student can self-enroll, it belongs here.
    Task<ServiceResult> EnrollInCourseAsync(Guid studentUserId, int courseId);
    Task<ServiceResult> UnenrollFromCourseAsync(Guid studentUserId, int courseId); // If allowed

    // --- Attendance Viewing ---
    Task<ServiceResult<IEnumerable<CourseAttendanceDetailsDto>>> GetMyAttendanceForCourseAsync(Guid studentUserId, int courseId);
    Task<ServiceResult<IEnumerable<CourseAttendanceDetailsDto>>> GetAllMyAttendanceAsync(Guid studentUserId); // Across all courses

    // --- Warnings and Notifications ---
    Task<ServiceResult<IEnumerable<WarningDto>>> GetMyWarningsAsync(Guid studentUserId); // Warnings issued to the student
    Task<ServiceResult<IEnumerable<NotificationDto>>> GetMyNotificationsAsync(Guid studentUserId, bool onlyUnread = false);
    Task<ServiceResult> MarkNotificationAsReadAsync(Guid studentUserId, int notificationId);
}