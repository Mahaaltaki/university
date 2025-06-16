using kalamon_University.DTOs.Auth;
using kalamon_University.DTOs.Admin;
using kalamon_University.DTOs.Common;
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using kalamon_University.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace kalamon_University.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IStudentRepository _studentRepository;
        private readonly IProfessorRepository _professorRepository;

        public UserService(
            UserManager<User> userManager,
            IStudentRepository studentRepository,
            IProfessorRepository professorRepository)
        {
            _userManager = userManager;
            _studentRepository = studentRepository;
            _professorRepository = professorRepository;
        }

        // 1. تسجيل مستخدم جديد
        public async Task<User?> AddUserAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return null;

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                Role = Enum.TryParse<Role>(dto.RoleName, out var parsedRole) ? parsedRole : Role.Student
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return null;

            await _userManager.AddToRoleAsync(user, user.Role.ToString());

            if (user.Role == Role.Student)
            {
                var student = new Student { UserId = user.Id };
                await _studentRepository.AddAsync(student);
                await _studentRepository.SaveChangesAsync();
            }
            else if (user.Role == Role.Professor)
            {
                var professor = new Professor
                {
                    UserId = user.Id,
                    Specialization = dto.Specialization
                };
                await _professorRepository.AddAsync(professor);
                await _professorRepository.SaveChangesAsync();
            }

            return user;
        }

        // 2. الحصول على مستخدم بالتفاصيل
        public async Task<UserDetailDto?> GetUserDetailedByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            string? specialization = null;

            if (user.Role == Role.Professor)
            {
                var prof = await _professorRepository.GetByUserIdAsync(user.Id);
                specialization = prof?.Specialization;
            }

            return new UserDetailDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToRoleName() ,
                Specialization = specialization
            };
        }

        // 3. الحصول على مستخدم عبر الإيميل
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // 4. الحصول على مستخدم عبر اسم المستخدم
        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        // 5. تعديل بيانات المستخدم
        public async Task<ServiceResult> UpdateUserProfileAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ServiceResult.Failed("User not found.");

            user.FullName = dto.FullName;
            // أضف أي حقول أخرى مسموحة بالتعديل

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? ServiceResult.Succeeded()
                : ServiceResult.Failed("Failed to update user.");
        }

        // 6. جلب جميع المستخدمين
        public async Task<IEnumerable<UserDetailDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDetailDto>();

            foreach (var user in users)
            {
                string? specialization = null;

                if (user.Role == Role.Professor)
                {
                    var prof = await _professorRepository.GetByUserIdAsync(user.Id);
                    specialization = prof?.Specialization;
                }

                userDtos.Add(new UserDetailDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role =  user.Role.ToRoleName() ,
                    Specialization = specialization
                });
            }

            return userDtos;
        }
    }
}