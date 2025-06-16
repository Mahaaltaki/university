using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kalamon_University.Interfaces;
using kalamon_University.DTOs.Admin;
using kalamon_University.DTOs.Auth;
using kalamon_University.DTOs.Course;
using kalamon_University.DTOs.Common;
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

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region User Management

        // POST: api/Admin/users
        [HttpPost("users")]
        [ProducesResponseType(typeof(UserDetailDto), 201)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                // تحويل أخطاء ModelState إلى ServiceResult
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ServiceResult.Failed(errors));
            }
            var result = await _adminService.CreateUserAsync(dto);
            return HandleResult(result, isCreation: true);
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

        // ... باقي دوال الـ Actions كما هي بدون تغيير ...
        // (Region for Role & Account Management, and Course Management)

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
        [AllowAnonymous] // للسماح بالوصول للصفحة قبل تسجيل الدخول
        [ApiExplorerSettings(IgnoreApi = true)] // لإخفاء هذه النقطة من Swagger لأنها ليست API
        public IActionResult AdminDashboard()
        {
            // سيعيد توجيه المتصفح إلى ملف admin.html الموجود في مجلد wwwroot
            return Redirect("/admin.html");
        }
    }
    // DTOs that might be needed for the controller
    public class AssignRoleDto { public string RoleName { get; set; } }
    public class ConfirmEmailDto { public bool IsConfirmed { get; set; } }
    public class AssignProfessorDto { public Guid ProfessorUserId { get; set; } }
}