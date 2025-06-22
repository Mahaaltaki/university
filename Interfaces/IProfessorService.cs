using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kalamon_University.DTOs.Attendance;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Notification;
using kalamon_University.DTOs.ProfessorPortal;
using kalamon_University.Models.Entities;

namespace kalamon_University.Interfaces
{
    /// <summary>
    /// æÇÌåÉ áÎÏãÇÊ ÅÏÇÑÉ ÇáÃÓÇÊĞÉ¡ ÊæİÑ ÇáÚãáíÇÊ ÇáÃÓÇÓíÉ (CRUD).
    /// </summary>
    public interface IProfessorService
    {
        /// <summary>
        /// ÌáÈ ŞÇÆãÉ ÈÌãíÚ ÇáÃÓÇÊĞÉ ãÚ ÈíÇäÇÊ ÇáãÓÊÎÏã ÇáãÑÊÈØÉ Èåã.
        /// </summary>
        /// <returns>ŞÇÆãÉ ÊÍÊæí Úáì ÌãíÚ ÇáÃÓÇÊĞÉ.</returns>
        Task<IEnumerable<Professor>> GetAllAsync();

        /// <summary>
        /// ÇáÈÍË Úä ÃÓÊÇĞ ãÍÏÏ ÈÇÓÊÎÏÇã ÇáãÚÑİ ÇáÎÇÕ Èå (UserId).
        /// </summary>
        /// <param name="professorId">ÇáãÚÑİ ÇáİÑíÏ (Guid) ááÃÓÊÇĞ.</param>
        /// <returns>ßÇÆä ÇáÃÓÊÇĞ ÅĞÇ Êã ÇáÚËæÑ Úáíå (ãÚ ÇáãæÇÏ ÇáÊí íÏÑÓåÇ)¡ æÅáÇ ÓíÚíÏ null.</returns>
        Task<Professor?> GetByIdAsync(Guid professorId);
        /// <summary>
        /// ÌáÈ ÌãíÚ ÇáßæÑÓÇÊ ÇáÊí íÏÑÓåÇ ÃÓÊÇĞ ãÚíä.
        /// </summary>
        Task<IEnumerable<ProfessorCourseDto>> GetMyCoursesAsync(Guid professorId);

        /// <summary>
        /// ÌáÈ ŞÇÆãÉ ÇáØáÇÈ ÇáãÓÌáíä İí ßæÑÓ ãÚíä íÏÑÓå ÇáÃÓÊÇĞ.
        /// </summary>
        Task<ServiceResult<IEnumerable<EnrolledStudentDto>>> GetStudentsInCourseAsync(Guid professorId, int courseId);

        /// <summary>
        /// ÌáÈ ÓÌá ÇáÍÖæÑ áÌãíÚ ÇáØáÇÈ İí ßæÑÓ ãÚíä íÏÑÓå ÇáÃÓÊÇĞ.
        /// </summary>
        Task<ServiceResult<IEnumerable<DTOs.Attendance.AttendanceRecordDto>>> GetAttendanceForCourseAsync(Guid professorId, int courseId);
        /// <summary>
        /// ÅÑÓÇá ÅÔÚÇÑ áÌãíÚ ÇáØáÇÈ ÇáãÓÌáíä İí ßæÑÓ ãÚíä.
        /// </summary>
        Task<ServiceResult> SendNotificationToCourseAsync(Guid professorId, int courseId, string message);

        /// <summary>
        /// ÌáÈ ÇáÅÔÚÇÑÇÊ ÇáÎÇÕÉ ÈÇáÃÓÊÇĞ.
        /// </summary>
        Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(Guid professorId);

        /// <summary>
        /// ÅÖÇİÉ ÓÌá ÍÖæÑ ÌÏíÏ áØÇáÈ İí ßæÑÓ ãÚíä.
        /// </summary>
        Task<ServiceResult<DTOs.Attendance.AttendanceRecordDto>> AddAttendanceAsync(Guid professorId, int courseId, CreateAttendanceDto dto);

        /// <summary>
        /// ÊÚÏíá ÓÌá ÍÖæÑ ãæÌæÏ.
        /// </summary>
        Task<ServiceResult> UpdateAttendanceAsync(Guid professorId, int courseId, int attendanceId, UpdateAttendanceDto dto);

        /// <summary>
        /// ÍĞİ ÓÌá ÍÖæÑ.
        /// </summary>
        Task<ServiceResult> DeleteAttendanceAsync(Guid professorId, int courseId, int attendanceId);

        // --- ÏÇáÉ ÊÕÏíÑ ÇáÅßÓá ---

        /// <summary>
        /// íæáÏ ÊŞÑíÑ ÇáÍÖæÑ ßãáİ ÅßÓá æíÑÓá ÅÔÚÇÑğÇ ááÜ Admins ãÚ ÑÇÈØ ÇáÊÍãíá.
        /// </summary>
        Task<ServiceResult> ExportAndNotifyAdminsAsync(Guid professorId, int courseId);
    }
}