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
    /// ����� ������ ����� �������ɡ ���� �������� �������� (CRUD).
    /// </summary>
    public interface IProfessorService
    {
        /// <summary>
        /// ��� ����� ����� �������� �� ������ �������� �������� ���.
        /// </summary>
        /// <returns>����� ����� ��� ���� ��������.</returns>
        Task<IEnumerable<Professor>> GetAllAsync();

        /// <summary>
        /// ����� �� ����� ���� �������� ������ ����� �� (UserId).
        /// </summary>
        /// <param name="professorId">������ ������ (Guid) �������.</param>
        /// <returns>���� ������� ��� �� ������ ���� (�� ������ ���� ������)� ���� ����� null.</returns>
        Task<Professor?> GetByIdAsync(Guid professorId);
        /// <summary>
        /// ��� ���� �������� ���� ������ ����� ����.
        /// </summary>
        Task<IEnumerable<ProfessorCourseDto>> GetMyCoursesAsync(Guid professorId);

        /// <summary>
        /// ��� ����� ������ �������� �� ���� ���� ����� �������.
        /// </summary>
        Task<ServiceResult<IEnumerable<EnrolledStudentDto>>> GetStudentsInCourseAsync(Guid professorId, int courseId);

        /// <summary>
        /// ��� ��� ������ ����� ������ �� ���� ���� ����� �������.
        /// </summary>
        Task<ServiceResult<IEnumerable<DTOs.Attendance.AttendanceRecordDto>>> GetAttendanceForCourseAsync(Guid professorId, int courseId);
        /// <summary>
        /// ����� ����� ����� ������ �������� �� ���� ����.
        /// </summary>
        Task<ServiceResult> SendNotificationToCourseAsync(Guid professorId, int courseId, string message);

        /// <summary>
        /// ��� ��������� ������ ��������.
        /// </summary>
        Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(Guid professorId);

        /// <summary>
        /// ����� ��� ���� ���� ����� �� ���� ����.
        /// </summary>
        Task<ServiceResult<DTOs.Attendance.AttendanceRecordDto>> AddAttendanceAsync(Guid professorId, int courseId, CreateAttendanceDto dto);

        /// <summary>
        /// ����� ��� ���� �����.
        /// </summary>
        Task<ServiceResult> UpdateAttendanceAsync(Guid professorId, int courseId, int attendanceId, UpdateAttendanceDto dto);

        /// <summary>
        /// ��� ��� ����.
        /// </summary>
        Task<ServiceResult> DeleteAttendanceAsync(Guid professorId, int courseId, int attendanceId);

        // --- ���� ����� ������ ---

        /// <summary>
        /// ���� ����� ������ ���� ���� ����� ������� ��� Admins �� ���� �������.
        /// </summary>
        Task<ServiceResult> ExportAndNotifyAdminsAsync(Guid professorId, int courseId);
    }
}