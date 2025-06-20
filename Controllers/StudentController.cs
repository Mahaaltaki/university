using kalamon_University.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System;
using kalamon_University.Services;
namespace kalamon_University.Controllers
{
    [Authorize(Roles = "Student")]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // دالة مساعدة لجلب معرف الطالب من التوكن أو الجلسة
        private Guid? GetCurrentStudentId() // غيّر نوع الإرجاع إلى Guid? القابل للفراغ
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                // إذا لم يتم العثور على الـ claim، أرجع null
                return null;
            }

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                // إذا نجح التحويل، أرجع الـ Guid
                return userId;
            }

            // إذا كان النص موجوداً ولكنه ليس Guid صالحاً، أرجع null
            return null;
        }

        // 1. واجهة لعرض جميع الكورسات المتاحة
        [HttpGet("courses/available")]
        public async Task<IActionResult> GetAvailableCourses()
        {
            var courses = await _studentService.GetAllAvailableCoursesAsync();
            return Ok(courses);
        }

        // 2. واجهة لعرض الكورسات المسجل بها الطالب
        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var studentIdNullable = GetCurrentStudentId();
            if (studentIdNullable == null)
            {
                // إذا لم نجد الـ ID، فهذا يعني أن التوكن غير صالح
                return Unauthorized("Invalid or missing user ID in token.");
            }
            var courses = await _studentService.GetMyEnrolledCoursesAsync(studentIdNullable.Value);
            return Ok(courses);
        }


        // 3. واجهة للتسجيل في كورس
        [HttpPost("courses/{courseId}/enroll")]
        public async Task<IActionResult> EnrollInCourse(int courseId)
        {
            var studentId = GetCurrentStudentId();

            // تحقق مما إذا تمكنا من جلب الـ ID
            if (studentId == null)
            {
                // أرجع خطأ يوضح أن هناك مشكلة في التوكن
                return Unauthorized("Invalid or missing user ID in token.");
            }

            // استخدم .Value لأن studentId الآن من نوع Guid?
            var result = await _studentService.EnrollInCourseAsync(studentId.Value, courseId);

            if (!result.Success) // افترض أن الخدمة ترجع ServiceResult
            {
                // استخدم الأخطاء من الخدمة لتقديم رسالة دقيقة
                return BadRequest(result);
            }

            return Ok(result);
        }

        // 4. واجهة لعرض جدول الحضور في كورس معين
        [HttpGet("courses/{courseId}/attendance")]
        public async Task<IActionResult> GetMyAttendance(int courseId)
        {
            var studentIdNullable = GetCurrentStudentId();

            
            if (studentIdNullable == null)
            {
                return Unauthorized("Invalid or missing user ID in token.");
            }
            var studentId = GetCurrentStudentId();
            var attendanceRecords = await _studentService.GetMyAttendanceForCourseAsync(studentIdNullable.Value, courseId);
            return Ok(attendanceRecords);
        }

        // 5. واجهة جديدة لعرض إشعارات الطالب
        [HttpGet("notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                return Unauthorized("Invalid or missing user ID in token.");
            }

            var notifications = await _studentService.GetMyNotificationsAsync(studentId.Value);

            return Ok(notifications);
        }
    }
}
