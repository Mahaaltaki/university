using kalamon_University.DTOs.Auth;
using kalamon_University.Models.Entities;
using kalamon_University.Models.Enums;
using kalamon_University.Interfaces; // استخدم Interfaces وليس Repository
using Microsoft.AspNetCore.Identity;
using kalamon_University.Repository;

namespace kalamon_University.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IStudentService _studentPortalService;
        private readonly IProfessorService _professorPortalService;

        public AuthService(
        UserManager<User> userManager,
            IStudentService studentPortalService,
            IProfessorService professorPortalService)
        {
            _userManager = userManager;
            _studentPortalService = studentPortalService;
            _professorPortalService = professorPortalService;
        }

        public async Task<User?> RegisterUserAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return null;

            var role = Enum.TryParse<Role>(dto.RoleName, out var parsedRole) ? parsedRole : Role.Student;

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                Role = role
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return null;

            await _userManager.AddToRoleAsync(user, role.ToString());

            if (role == Role.Student)
            {
                var student = new Student { UserId = user.Id };
                await _studentPortalService.AddAsync(student);
                await _studentPortalService.SaveChangesAsync();
            }
            else if (role == Role.Professor)
            {
                var professor = new Professor
                {
                    UserId = user.Id,
                    Specialization = dto.Specialization
                };
                await _professorPortalService.AddAsync(professor);
                await _professorPortalService.SaveChangesAsync();
            }

            return user;
        }
    }
}
