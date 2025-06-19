// Kalamon_University/Services/AdminService.cs

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using kalamon_University.DTOs.Admin;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Auth;
using kalamon_University.DTOs.Course;
using kalamon_University.Models.Entities;
using kalamon_University.Interfaces;
using kalamon_University.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace kalamon_University.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IStudentRepository _studentRepository;
        private readonly IProfessorRepository _professorRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly AppDbContext _context; // AppDbContext is often needed for transactions or complex queries
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IStudentRepository studentRepository,
            IProfessorRepository professorRepository,
            ICourseRepository courseRepository,
            AppDbContext context,
            IMapper mapper,
            ILogger<AdminService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _studentRepository = studentRepository;
            _professorRepository = professorRepository;
            _courseRepository = courseRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // --- User Profile Management ---

        public async Task<ServiceResult> CreateStudentProfileAsync(Guid userId)
        {
            _logger.LogInformation("Attempting to create a student profile for UserID {UserId}", userId);

            // 1. Check if the user exists
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Profile creation failed: User with ID {UserId} not found.", userId);
                return ServiceResult.Failed("User not found.");
            }

            // 2. Check if a student profile already exists for this user
            var existingProfile = await _studentRepository.GetByUserIdAsync(userId);
            if (existingProfile != null)
            {
                _logger.LogWarning("Profile creation failed: Student profile for UserID {UserId} already exists.", userId);
                return ServiceResult.Failed("Student profile already exists for this user.");
            }

            // 3. Create and save the new student profile
            var studentProfile = new Student { UserId = userId };

            try
            {
                await _studentRepository.AddAsync(studentProfile);
                await _studentRepository.SaveChangesAsync();
                _logger.LogInformation("Successfully created student profile for UserID {UserId}", userId);
                return ServiceResult.Succeeded("Student profile created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student profile for UserID {UserId}", userId);
                return ServiceResult.Failed("An unexpected error occurred while creating the student profile.");
            }
        }

        public async Task<ServiceResult> CreateProfessorProfileAsync(Guid userId, string specialization)
        {
            _logger.LogInformation("Attempting to create a professor profile for UserID {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Profile creation failed: User with ID {UserId} not found.", userId);
                return ServiceResult.Failed("User not found.");
            }

            var existingProfile = await _professorRepository.GetByUserIdAsync(userId);
            if (existingProfile != null)
            {
                _logger.LogWarning("Profile creation failed: Professor profile for UserID {UserId} already exists.", userId);
                return ServiceResult.Failed("Professor profile already exists for this user.");
            }

            var professorProfile = new Professor
            {
                UserId = userId,
                Specialization = specialization ?? "Not specified"
            };

            try
            {
                await _professorRepository.AddAsync(professorProfile);
                await _professorRepository.SaveChangesAsync();
                _logger.LogInformation("Successfully created professor profile for UserID {UserId}", userId);
                return ServiceResult.Succeeded("Professor profile created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating professor profile for UserID {UserId}", userId);
                return ServiceResult.Failed("An unexpected error occurred while creating the professor profile.");
            }
        }

        // --- User Information Management ---

        public async Task<ServiceResult<UserDetailDto>> GetUserDetailsByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return ServiceResult<UserDetailDto>.Failed("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            var dto = _mapper.Map<UserDetailDto>(user);
            dto.Role = userRole;

            if (userRole == "Student")
            {
                var student = await _studentRepository.GetByUserIdAsync(user.Id);
                if (student != null) dto.StudentProfileId = student.UserId;
            }
            else if (userRole == "Professor")
            {
                var prof = await _professorRepository.GetByUserIdAsync(user.Id);
                if (prof != null)
                {
                    dto.ProfessorProfileId = prof.UserId;
                    dto.Specialization = prof.Specialization;
                }
            }

            return ServiceResult<UserDetailDto>.Succeeded(dto);
        }

        public async Task<ServiceResult<IEnumerable<UserDetailDto>>> GetAllUsersAsync(string? roleFilter = null)
        {
            IQueryable<User> usersQuery = _userManager.Users;

            if (!string.IsNullOrEmpty(roleFilter))
            {
                // This is more efficient than getting all users and then filtering in-memory
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleFilter);
                var userIdsInRole = usersInRole.Select(u => u.Id);
                usersQuery = usersQuery.Where(u => userIdsInRole.Contains(u.Id));
            }

            var users = await usersQuery
                .Include(u => u.ProfessorProfile) // Eager load profiles
                .Include(u => u.StudentProfile)
                .ToListAsync();

            var userDtos = new List<UserDetailDto>();
            foreach (var user in users)
            {
                var dto = _mapper.Map<UserDetailDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                dto.Role = roles.FirstOrDefault();

                // Mapping from eager-loaded data is faster
                if (user.ProfessorProfile != null) dto.Specialization = user.ProfessorProfile.Specialization;

                userDtos.Add(dto);
            }

            return ServiceResult<IEnumerable<UserDetailDto>>.Succeeded(userDtos);
        }

        public async Task<ServiceResult<UserDetailDto>> UpdateUserAsync(Guid userId, UpdateUserDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return ServiceResult<UserDetailDto>.Failed("User not found.");
            }

            user.FullName = updateDto.FullName ?? user.FullName;
            user.Email = updateDto.Email ?? user.Email;
            user.UserName = updateDto.Email ?? user.UserName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return ServiceResult<UserDetailDto>.Failed(updateResult.Errors.Select(e => e.Description).ToList());
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Professor"))
            {
                var professor = await _professorRepository.GetByUserIdAsync(user.Id);
                if (professor != null && updateDto.Specialization != null)
                {
                    professor.Specialization = updateDto.Specialization;
                    await _professorRepository.SaveChangesAsync();
                }
            }

            return await GetUserDetailsByIdAsync(userId);
        }

        public async Task<ServiceResult> DeleteUserAsync(Guid userId)
        {
            _logger.LogInformation("Attempting to delete user with UserID {AppUserId}", userId);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User deletion failed: User with ID {AppUserId} not found.", userId);
                return ServiceResult.Failed("User not found.");
            }

            // Deleting the user will trigger cascade delete on the profile if configured in the database
            var identityResult = await _userManager.DeleteAsync(user);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToList();
                _logger.LogWarning("User deletion failed for UserID {AppUserId}. Errors: {Errors}", userId, string.Join(", ", errors));
                return ServiceResult.Failed(errors);
            }

            _logger.LogInformation("Successfully deleted user with UserID {AppUserId}", userId);
            return ServiceResult.Succeeded("User deleted successfully.");
        }

        // ... (Keep the rest of the methods like AssignRoleToUserAsync, Course Management, etc. as they are)

        // --- Role & Account Management ---

        public async Task<ServiceResult> AssignRoleToUserAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return ServiceResult.Failed("User not found");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return ServiceResult.Failed($"Role '{roleName}' does not exist.");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult.Failed(errors);
            }
            return ServiceResult.Succeeded("Role assigned successfully.");
        }


        public async Task<ServiceResult> ConfirmUserEmailByAdminAsync(Guid userId, bool confirmedStatus)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ServiceResult.Failed("User not found.");

            user.EmailConfirmed = confirmedStatus;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult.Failed(errors);
            }
            return ServiceResult.Succeeded("Email confirmation status updated successfully.");
        }

        // --- Course Management by Admin ---
        public async Task<ServiceResult<CourseDetailDto>> CreateCourseAsync(CreateCourseDto courseDto)
        {
            // الخطوة 1: التحقق من وجود البروفيسور (إذا تم توفير ID)
            if (courseDto.ProfessorId != Guid.Empty) // تحقق مما إذا كان الـ Guid ليس القيمة الافتراضية الفارغة
            {
                var professorExists = await _context.Professors
                                                .AnyAsync(p => p.UserId == courseDto.ProfessorId);

                if (!professorExists)
                {
                    // إرجاع خطأ واضح للمستخدم بدلاً من خطأ 500
                    return ServiceResult<CourseDetailDto>.Failed("Professor with the specified ID was not found.");
                }
            }

            // الخطوة 2: إنشاء الكورس (بعد التأكد من صحة البيانات)
            var course = _mapper.Map<Course>(courseDto);
            course.TotalHours = course.PracticalHours + course.TheoreticalHours;

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();

            var resultDto = _mapper.Map<CourseDetailDto>(course);
            // يمكنك تحسين هذا لجلب اسم الأستاذ مباشرة بعد الإنشاء إذا لزم الأمر
            return ServiceResult<CourseDetailDto>.Succeeded(resultDto, "Course created successfully.");
        }


        public async Task<ServiceResult<CourseDetailDto>> GetCourseByIdAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId, includeProfessor: true);
            if (course == null)
            {
                return ServiceResult<CourseDetailDto>.Failed("Course not found.");
            }
            var dto = _mapper.Map<CourseDetailDto>(course);
            return ServiceResult<CourseDetailDto>.Succeeded(dto);
        }


        public async Task<ServiceResult<IEnumerable<CourseDetailDto>>> GetAllCoursesAsync(bool includeProfessorDetails = false)
        {
            var courses = await _courseRepository.GetAllCoursesAsync(includeProfessorDetails);
            var courseDtos = _mapper.Map<IEnumerable<CourseDetailDto>>(courses);
            return ServiceResult<IEnumerable<CourseDetailDto>>.Succeeded(courseDtos);
        }


        public async Task<ServiceResult> UpdateCourseAsync(int courseId, UpdateCourseDto courseDto)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return ServiceResult.Failed("Course not found.");
            }

            // الخطوة 1: التحقق من وجود البروفيسور الجديد قبل التعيين
            // نفترض أن ProfessorId في UpdateCourseDto هو Guid? (nullable)
            if (courseDto.ProfessorId.HasValue && courseDto.ProfessorId.Value != Guid.Empty)
            {
                // تحقق فقط إذا تم إرسال ID جديد وصالح
                var professorExists = await _context.Professors
                                                .AnyAsync(p => p.UserId == courseDto.ProfessorId.Value);

                if (!professorExists)
                {
                    // أرجع خطأ واضحاً
                    return ServiceResult.Failed("The new Professor ID provided was not found.");
                }
            }

            // الخطوة 2: تحديث بيانات الكورس بعد التحقق
            // AutoMapper سيقوم بنسخ كل الخصائص بما فيها ProfessorId الصالح (أو null)
            _mapper.Map(courseDto, course);
            course.TotalHours = course.PracticalHours + course.TheoreticalHours;

            _courseRepository.Update(course);
            await _courseRepository.SaveChangesAsync();

            return ServiceResult.Succeeded("Course updated successfully.");
        }


        public async Task<ServiceResult> DeleteCourseAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return ServiceResult.Failed("Course not found.");
            }
            _courseRepository.Delete(course);
            await _courseRepository.SaveChangesAsync();
            return ServiceResult.Succeeded("Course deleted successfully.");
        }


        public async Task<ServiceResult> AssignProfessorToCourseAsync(int courseId, Guid professorUserId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return ServiceResult.Failed("Course not found.");
            }

            var professor = await _professorRepository.GetByUserIdAsync(professorUserId);
            if (professor == null)
            {
                return ServiceResult.Failed("Professor not found.");
            }

            course.ProfessorId = professor.UserId;
            _courseRepository.Update(course);
            await _courseRepository.SaveChangesAsync();
            return ServiceResult.Succeeded("Professor assigned to course successfully.");
        }

        public async Task<ServiceResult> UnassignProfessorFromCourseAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return ServiceResult.Failed("Course not found.");

            course.ProfessorId = null;

            _courseRepository.Update(course);
            await _courseRepository.SaveChangesAsync();

            return ServiceResult.Succeeded("Professor unassigned from course successfully.");
        }



    }
}