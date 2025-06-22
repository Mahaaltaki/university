using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kalamon_University.Interfaces;
using kalamon_University.DTOs.Admin;
using kalamon_University.DTOs.Auth;
using kalamon_University.DTOs.Course;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Notification;
using Microsoft.EntityFrameworkCore;
using kalamon_University.Data;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using System.Collections.Generic; // Required for IEnumerable
using System.Linq; // Required for .Select()

namespace kalamon_University.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;
        private readonly IFileStorageService _fileStorage;
        private readonly AppDbContext _context; // ستحتاج لحقن سياق قاعدة البيانات

        public AdminController(IAdminService adminService, IAuthService authService , IFileStorageService fileStorage, AppDbContext context)
        {
            _adminService = adminService;
            _authService = authService;
            _fileStorage = fileStorage;
            _context = context;
        }// دالة مساعدة لجلب معرف المسؤول الحالي
        private Guid? GetCurrentAdminId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return null;
        }

        #region User Management

        //عرض المستخدمين بناء على الدور 
        // GET: api/Admin/users
        [HttpGet("users")]
        [ProducesResponseType(typeof(IEnumerable<UserDetailDto>), 200)]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? role)
        {
            var result = await _adminService.GetAllUsersAsync(role);
            return HandleResult(result);
        }

        // POST: api/Admin/users
        [HttpPost("users")]
        [ProducesResponseType(typeof(UserDetailDto), 201)]
        [ProducesResponseType(typeof(AuthResultDto), 400)]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                // تحويل أخطاء ModelState إلى ServiceResult
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new AuthResultDto { Succeeded = false, Errors = errors });
            }
            var result = await _authService.RegisterAsync(dto);
            if (result.Succeeded)
            {
                // عند النجاح، RegisterAsync تُرجع بيانات المستخدم والتوكن
                // يمكنك إرجاع بيانات المستخدم فقط إذا أردت
                return CreatedAtAction(nameof(GetUserById), new { userId = result.User.Id }, result.User);
            }

            return BadRequest(result); // result هنا هو من نوع AuthResultDto ويحتوي على الأخطاء
        }

        // GET: api/Admin/users/{userId}
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(UserDetailDto), 200)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var result = await _adminService.GetUserDetailsByIdAsync(userId);
            return HandleResult(result);
        }

        // PUT: api/Admin/users/{userId}
        [HttpPut("users/{userId}")]
        [ProducesResponseType(typeof(UserDetailDto), 200)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ServiceResult.Failed(errors));
            }
            var result = await _adminService.UpdateUserAsync(userId, dto);
            return HandleResult(result);
        }

        // DELETE: api/Admin/users/{userId}
        [HttpDelete("users/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            return HandleResult(result, noContentOnSuccess: true);
        }

        #endregion

        
        #region Role & Account Management

        // POST: api/Admin/users/{userId}/assign-role
        [HttpPost("users/{userId}/assign-role")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        public async Task<IActionResult> AssignRoleToUser(Guid userId, [FromBody] AssignRoleDto dto)
        {
            var result = await _adminService.AssignRoleToUserAsync(userId, dto.RoleName);
            return HandleResult(result);
        }

        // POST: api/Admin/users/{userId}/confirm-email
        [HttpPost("users/{userId}/confirm-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        public async Task<IActionResult> ConfirmUserEmail(Guid userId, [FromBody] ConfirmEmailDto dto)
        {
            var result = await _adminService.ConfirmUserEmailByAdminAsync(userId, dto.IsConfirmed);
            return HandleResult(result);
        }

        #endregion

        // POST: api/Admin/courses
        [HttpPost("courses")]
        [ProducesResponseType(typeof(CourseDetailDto), 201)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            var result = await _adminService.CreateCourseAsync(dto);
            return HandleResult(result, isCreation: true);
        }

        // GET: api/Admin/courses
        [HttpGet("courses")]
        [ProducesResponseType(typeof(IEnumerable<CourseDetailDto>), 200)]
        public async Task<IActionResult> GetAllCourses([FromQuery] bool includeProfessorDetails = false)
        {
            var result = await _adminService.GetAllCoursesAsync(includeProfessorDetails);
            return HandleResult(result);
        }

        // GET: api/Admin/courses/{courseId}
        [HttpGet("courses/{courseId}")]
        [ProducesResponseType(typeof(CourseDetailDto), 200)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> GetCourseById(int courseId)
        {
            var result = await _adminService.GetCourseByIdAsync(courseId);
            return HandleResult(result);
        }

        // PUT: api/Admin/courses/{courseId}
        [HttpPut("courses/{courseId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] UpdateCourseDto dto)
        {
            var result = await _adminService.UpdateCourseAsync(courseId, dto);
            return HandleResult(result);
        }

        // DELETE: api/Admin/courses/{courseId}
        [HttpDelete("courses/{courseId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var result = await _adminService.DeleteCourseAsync(courseId);
            return HandleResult(result, noContentOnSuccess: true);
        }

        // POST: api/Admin/courses/{courseId}/assign-professor
        [HttpPost("courses/{courseId}/assign-professor")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> AssignProfessorToCourse(int courseId,  AssignProfessorDto dto)
        {
            var result = await _adminService.AssignProfessorToCourseAsync(courseId, dto.ProfessorUserId);
           
            return HandleResult(result);
        }

        // DELETE: api/Admin/courses/{courseId}/unassign-professor
        [HttpDelete("courses/{courseId}/unassign-professor")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> UnassignProfessorFromCourse(int courseId)
        {
            var result = await _adminService.UnassignProfessorFromCourseAsync(courseId);
            return HandleResult(result);
        }


        #region Helper Method 

        private IActionResult HandleResult<T>(ServiceResult<T> result, bool isCreation = false)
        {
            if (result.Success) 
            {
                if (isCreation)
                {
                    // For POST requests that create a resource
                    return CreatedAtAction(null, result.Data); // 201 Created
                }
                return Ok(result.Data); // 200 OK
            }

            // Check if any error message suggests a "not found" scenario
            if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
            {
                return NotFound(result); // 404 Not Found
            }
            return BadRequest(result); // 400 Bad Request
        }

        private IActionResult HandleResult(ServiceResult result, bool noContentOnSuccess = false)
        {
            if (result.Success) 
            {
                if (noContentOnSuccess)
                {
                    return NoContent(); // 204 No Content for successful DELETE
                }
                return Ok(result); // 200 OK
            }

            // Check if any error message suggests a "not found" scenario
            if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        #endregion

        // GET: api/Admin/dashboard
        [HttpGet("dashboard")]
        [AllowAnonymous] // للسماح بالوصول للصفحة 
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult AdminDashboard()
        {
            // سيعيد توجيه المتصفح إلى ملف admin.html الموجود في مجلد wwwroot
            return Redirect("/admin.html");
        }
        [HttpGet("notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var adminId = GetCurrentAdminId();
            if (adminId == null)
            {
                return Unauthorized();
            }

            // الخطوة 1: جلب البيانات من قاعدة البيانات كـ List<Notification>
            var notificationEntities = await _context.Notifications
                .Where(n => n.UserId == adminId.Value)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking() // <-- إضافة جيدة لتحسين الأداء للقراءة فقط
                .ToListAsync(); // <-- استدعاء ToListAsync هنا على كيان قاعدة البيانات

            // الخطوة 2: تحويل القائمة الناتجة إلى List<NotificationDto> في الذاكرة
            var notificationsDto = notificationEntities.Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead,
                RelatedEntityType = n.RelatedEntityType,
                RelatedEntityName = n.RelatedEntityName
            }).ToList(); // <-- استخدم .ToList() العادية هنا لأننا نعمل في الذاكرة

            return Ok(notificationsDto);
        }

    }
    // DTOs that might be needed for the controller
    public class AssignRoleDto { public string RoleName { get; set; } }
    public class ConfirmEmailDto { public bool IsConfirmed { get; set; } }
    public class AssignProfessorDto { public Guid ProfessorUserId { get; set; } }
}