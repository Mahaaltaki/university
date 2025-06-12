// File: kalamon_University/Controllers/AuthController.cs
using kalamon_University.DTOs.Auth;
using kalamon_University.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace kalamon_University.Controllers
{
    [Route("api/[controller]")] // المسار الأساسي: /api/Auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger; // إضافة Logger

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user (Student or Professor).
        /// </summary>
        /// <remarks>
        /// Provide user details including role ("Student" or "Professor").
        /// If "Student", StudentIdNumber is required.
        /// If "Professor", StaffIdNumber is required.
        /// </remarks>
        /// <param name="model">Registration data.</param>
        /// <returns>Authentication result with token and user details on success.</returns>
        [HttpPost("register")] // POST /api/Auth/register
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)] // إذا نجح ولكن ربما بإرجاع بيانات بدلاً من 201
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status201Created)] // للإنشاء الناجح
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                // على الرغم من أن AuthService قد يرجع AuthResultDto مع أخطاء،
                // من الجيد إرجاع أخطاء ModelState مباشرة هنا إذا كانت بسيطة.
                // لكن بما أن AuthService يرجع AuthResultDto، يمكننا الاعتماد عليه.
                // return BadRequest(ModelState);
            }

            _logger.LogInformation("POST /api/Auth/register called for email: {Email}", model.Email);
            var result = await _authService.RegisterAsync(model);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Registration failed for email {Email}. Errors: {Errors}", model.Email, string.Join(", ", result.Errors));
                return BadRequest(result); // يرجع AuthResultDto مع الأخطاء وحالة Succeeded = false
            }

            _logger.LogInformation("Registration successful for email {Email}. User ID: {UserId}", model.Email, result.User?.Id);
            // عادةً ما يكون 201 Created مناسبًا لعملية تسجيل ناجحة مع إرجاع بيانات.
            // يمكنك إرجاع مسار للمستخدم المنشأ إذا أردت، ولكن هنا نرجع الـ AuthResultDto مباشرة.
            return CreatedAtAction(nameof(Login), result); // أو Ok(result) إذا كنت لا تريد CreatedAtAction
        }

        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="model">Login credentials (email and password).</param>
        /// <returns>Authentication result with token and user details on success.</returns>
        [HttpPost("login")] // POST /api/Auth/login
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                // return BadRequest(ModelState);
            }
            _logger.LogInformation("POST /api/Auth/login called for email: {Email}", model.Email);
            var result = await _authService.LoginAsync(model);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed for email {Email}. Errors: {Errors}", model.Email, string.Join(", ", result.Errors));
                return Unauthorized(result); // 401 Unauthorized هو الأنسب لفشل تسجيل الدخول بسبب بيانات اعتماد غير صحيحة
            }

            _logger.LogInformation("Login successful for email {Email}. User ID: {UserId}", model.Email, result.User?.Id);
            return Ok(result);
        }

        // يمكنك إضافة نقاط نهاية أخرى متعلقة بالمصادقة هنا إذا لزم الأمر، مثل:
        // - /refresh-token
        // - /forgot-password
        // - /reset-password
        // - /confirm-email
        // - /change-password (إذا كان المستخدم مسجلاً دخوله)
    }
}