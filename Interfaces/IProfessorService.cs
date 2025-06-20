using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task<ServiceResult<IEnumerable<AttendanceRecordDto>>> GetAttendanceForCourseAsync(Guid professorId, int courseId);

        /// <summary>
        /// ����� ����� ����� ������ �������� �� ���� ����.
        /// </summary>
        Task<ServiceResult> SendNotificationToCourseAsync(Guid professorId, int courseId, string message);

        /// <summary>
        /// ��� ��������� ������ ��������.
        /// </summary>
        Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(Guid professorId);
       
    }
}