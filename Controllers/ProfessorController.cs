
using kalamon_University.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using kalamon_University.DTOs.ProfessorPortal; 

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
    }
}