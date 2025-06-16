using kalamon_University.DTOs.Admin;
using kalamon_University.DTOs.Auth;
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using kalamon_University.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // مهم جداً لإحضار البيانات المرتبطة (Eager Loading)
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace kalamon_University.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStudentService _studentPortalService;
        private readonly IProfessorService _professorPortalService;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IStudentService studentPortalService,
            IProfessorService professorPortalService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _studentPortalService = studentPortalService;
            _professorPortalService = professorPortalService;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return new AuthResultDto { Succeeded = false, Errors = new List<string> { "هذا البريد الإلكتروني مسجل بالفعل." } };
            }

            if (!Enum.TryParse<Role>(dto.RoleName, true, out var roleEnum))
            {
                return new AuthResultDto { Succeeded = false, Errors = new List<string> { "الدور المحدد غير صالح." } };
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                UserName = dto.Email.ToLower(),
                Role = roleEnum
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResultDto { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };
            }

            await _userManager.AddToRoleAsync(user, roleEnum.ToString());

            try
            {
                if (roleEnum == Role.Student)
                {
                    await _studentPortalService.AddAsync(new Student { UserId = user.Id });
                }
                else if (roleEnum == Role.Professor)
                {
                    await _professorPortalService.AddAsync(new Professor { UserId = user.Id, Specialization = dto.Specialization ?? "غير محدد" });
                }
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                return new AuthResultDto { Succeeded = false, Errors = new List<string> { "فشل إنشاء الملف الشخصي: " + ex.Message } };
            }

            // عند نجاح التسجيل، قم بتسجيل الدخول مباشرة وإرجاع التوكن وبيانات المستخدم
            return await LoginAsync(new LoginDto { Email = dto.Email, Password = dto.Password });
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            // نستخدم Include لجلب بيانات البروفايل مع المستخدم
            var user = await _userManager.Users
                .Include(u => u.ProfessorProfile) // إحضار بيانات الأستاذ
                .SingleOrDefaultAsync(u => u.NormalizedEmail == dto.Email.ToUpper());

            if (user == null)
            {
                return new AuthResultDto { Succeeded = false, Errors = new List<string> { "البريد الإلكتروني أو كلمة المرور غير صحيحة." } };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new AuthResultDto { Succeeded = false, Errors = new List<string> { "البريد الإلكتروني أو كلمة المرور غير صحيحة." } };
            }

            var token = await GenerateJwtToken(user);
            var userDetail = MapUserToDetailDto(user);

            return new AuthResultDto { Succeeded = true, Token = token, User = userDetail };
        }

        private UserDetailDto MapUserToDetailDto(User user)
        {
            return new UserDetailDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                // جلب التخصص فقط إذا كان المستخدم أستاذًا ولديه ملف شخصي
                Specialization = user.ProfessorProfile?.Specialization
            };
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FullName", user.FullName) // إضافة اسم المستخدم الكامل كـ claim
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key not configured in appsettings.json");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}