namespace kalamon_University.Interfaces
using kalamon_University.DTOs.Course; // For CourseDto, StudentInCourseDto
using kalamon_University.DTOs.Attendance; // For AttendanceRecordDto, RecordAttendanceDto
using kalamon_University.DTOs.Common; // For ServiceResult or similar

// --- DTOs that might be specific to Professor Portal or reused ---
// (Define these in DTOs/ProfessorPortal/ or relevant folders)
// namespace kalamon_University.DTOs.ProfessorPortal;
// public record ProfessorProfileDto(Guid ProfessorId, string Name, string Email, string Specialization);
// public record UpdateProfessorProfileDto(string? Name, string? Specialization);
// public record CourseTaughtDto (int CourseId, string CourseName);
// public record StudentAttendanceSummaryDto (Guid StudentId, string StudentName, int PresentCount, int AbsentCount);

namespace UniversityApi.Core.Interfaces.Services;

using kalamon_University.DTOs.Common; // For ServiceResult
using kalamon_University.DTOs.Course; // For CourseDto, StudentInCourseDto etc.
using kalamon_University.DTOs.Attendance; // For Attendance DTOs
using kalamon_University.DTOs.ProfessorPortal; // For Professor specific DTOs
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;

public interface IProfessorPortalService
{
    // --- Profile Management ---
    Task<ServiceResult<ProfessorProfileDto>> GetProfessorProfileAsync(Guid ProfessorUserId); // ProfessorUserId is User.Id
    Task<ServiceResult> UpdateProfessorProfileAsync(Guid ProfessorUserId, UpdateProfessorProfileDto profileDto);

    // --- Course Management (View only for Professor, creation/assignment by Admin) ---
    Task<ServiceResult<IEnumerable<CourseTaughtDto>>> GetTaughtCoursesAsync(Guid ProfessorUserId);
    Task<ServiceResult<CourseDto>> GetCourseDetailsAsync(int courseId, Guid ProfessorUserId); // Ensure Professor teaches this course
    Task<ServiceResult<IEnumerable<StudentInCourseDto>>> GetStudentsInCourseAsync(int courseId, Guid ProfessorUserId);

    // --- Attendance Management ---
    Task<ServiceResult> RecordStudentAttendanceAsync(Guid ProfessorUserId, RecordAttendanceDto attendanceDto); // Professor records attendance for a student in a session
    Task<ServiceResult> BulkRecordAttendanceAsync(Guid ProfessorUserId, int courseId, DateTime sessionDate, IEnumerable<StudentAttendanceStatusDto> studentStatuses); // For recording attendance for multiple students
    Task<ServiceResult<IEnumerable<AttendanceRecordDto>>> GetAttendanceForCourseSessionAsync(int courseId, DateTime sessionDate, Guid doctorUserId);
    Task<ServiceResult<IEnumerable<StudentAttendanceSummaryDto>>> GetAttendanceSummaryForCourseAsync(int courseId, Guid ProfessorUserId);

    // --- Excel related for Attendance ---
    // GenerateAttendanceSheetForCourseAsync is a good candidate here or in a shared IExcelProcessingService
    // ParseAndRecordAttendanceFromExcelAsync could also be here, taking doctorUserId for validation.
    Task<ServiceResult<byte[]>> GenerateAttendanceSheetForCourseAsync(int courseId, Guid doctorUserId);
    Task<ServiceResult> ProcessUploadedAttendanceSheetAsync(int courseId, Guid doctorUserId, Stream excelStream);


    // --- Warnings/Notifications (related to Professor's students/courses) ---
    // Task<ServiceResult> IssueWarningToStudentAsync(Guid ProfessorUserId, IssueWarningDto warningDto); // Professor might issue some types of warnings
    // Task<ServiceResult<IEnumerable<NotificationDto>>> GetProfessorNotificationsAsync(Guid ProfessorUserId); // Notifications targeted to the Professor
}