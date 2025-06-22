
using kalamon_University.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using kalamon_University.DTOs.ProfessorPortal;
using kalamon_University.Models.Entities;

namespace kalamon_University.Controllers
{
    [Authorize(Roles = "Professor")] // <-- هام جداً: حماية الكونترولر للأساتذة فقط
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _professorService;

        public ProfessorController(IProfessorService professorService)
        {
            _professorService = professorService;
        }

        // دالة مساعدة لجلب معرف الأستاذ من التوكن
        private Guid? GetCurrentProfessorId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return null;
        }

        // 1. واجهة لعرض الكورسات التي يدرسها الأستاذ
        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized();

            var courses = await _professorService.GetMyCoursesAsync(professorId.Value);
            return Ok(courses);
        }

        // 2. واجهة لعرض الطلاب المسجلين في كورس معين
        [HttpGet("courses/{courseId}/students")]
        public async Task<IActionResult> GetStudentsInCourse(int courseId)
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized();

            var result = await _professorService.GetStudentsInCourseAsync(professorId.Value, courseId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        // 3. واجهة لعرض جدول الحضور لكورس معين
        [HttpGet("courses/{courseId}/attendance")]
        public async Task<IActionResult> GetCourseAttendance(int courseId)
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized();

            var result = await _professorService.GetAttendanceForCourseAsync(professorId.Value, courseId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        // 4. واجهة لإرسال إشعار لطلاب كورس معين
        [HttpPost("courses/{courseId}/notify")]
        public async Task<IActionResult> NotifyCourseStudents(int courseId, [FromBody] SendNotificationDto dto)
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized();

            var result = await _professorService.SendNotificationToCourseAsync(professorId.Value, courseId, dto.Message);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // 5. واجهة لعرض إشعارات الأستاذ
        [HttpGet("notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized();

            var notifications = await _professorService.GetMyNotificationsAsync(professorId.Value);
            return Ok(notifications);
        }
        // --- Endpoints للتحكم بالحضور ---

        // 6. إضافة سجل حضور جديد
        [HttpPost("courses/{courseId}/Add-attendance")]
        public async Task<IActionResult> AddAttendance(int courseId, [FromBody] DTOs.Attendance.CreateAttendanceDto dto)
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized();

            var result = await _professorService.AddAttendanceAsync(professorId.Value, courseId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetCourseAttendance), new { courseId = courseId }, result.Data);
        }

        // 7. تعديل سجل حضور
        [HttpPut("courses/{courseId}/attendance/{attendanceId}")]
        public async Task<IActionResult> UpdateAttendance(int courseId, int attendanceId, [FromBody] DTOs.Attendance.UpdateAttendanceDto dto)
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized();

            var result = await _professorService.UpdateAttendanceAsync(professorId.Value, courseId, attendanceId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return NoContent(); // أو Ok(result)
        }

        // 8. حذف سجل حضور
        [HttpDelete("courses/{courseId}/attendance/{attendanceId}")]
        public async Task<IActionResult> DeleteAttendance(int courseId, int attendanceId)
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized();

            var result = await _professorService.DeleteAttendanceAsync(professorId.Value, courseId, attendanceId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return NoContent();
        }

        // --- Endpoint لتصدير الإكسل ---

        // 9. تصدير وإرسال تقرير الحضور كإشعار للمسؤولين
        #region Notifications & Reporting

  

        // POST: api/professor/courses/{courseId}/attendance/export-and-notify
        [HttpPost("courses/{courseId}/attendance/export-and-notify")]
        public async Task<IActionResult> ExportAndNotifyAdmins(int courseId)
        {
            var professorId = GetCurrentProfessorId();
            if (professorId == null) return Unauthorized("Invalid token.");

            var result = await _professorService.ExportAndNotifyAdminsAsync(professorId.Value, courseId);

            if (!result.Success)
            {
                return BadRequest(result); // أو BadRequest(result.Errors)
            }
            return Ok(result);
        }

        #endregion
    }
}