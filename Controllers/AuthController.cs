using Microsoft.AspNetCore.Mvc;
using kalamon_University.DTOs.Auth;
using kalamon_University.Interfaces; // استيراد الواجهة
using System.Threading.Tasks;
using System.Linq; // لاستخدام .Select() في حال وجود أخطاء في ModelState

namespace kalamon_University.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // الاعتماد على الواجهة IAuthService بدلاً من الكلاس AuthService
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // أولاً، تحقق من صحة البيانات المدخلة (مثل [Required], [EmailAddress])
            if (!ModelState.IsValid)
            {
                // إرجاع قائمة بالأخطاء إذا كانت البيانات المدخلة غير صالحة
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new AuthResultDto { Succeeded = false, Errors = errors });
            }

            // استدعاء خدمة التسجيل
            var result = await _authService.RegisterAsync(dto);

            // إذا فشلت العملية، أرجع خطأ 400 Bad Request مع تفاصيل الأخطاء
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            // إذا نجحت العملية، أرجع 200 OK مع التوكن وبيانات المستخدم
            return Ok(result);
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new AuthResultDto { Succeeded = false, Errors = errors });
            }

            // استدعاء خدمة تسجيل الدخول
            var result = await _authService.LoginAsync(dto);

            // إذا فشلت العملية، أرجع خطأ 401 Unauthorized
            // هذا هو الرمز الأنسب لفشل المصادقة
            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }

            // إذا نجحت العملية، أرجع 200 OK مع التوكن وبيانات المستخدم
            return Ok(result);
        }
    }
}