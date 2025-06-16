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

namespace kalamon_University.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IStudentRepository _studentRepository;
        private readonly IProfessorRepository _professorRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly AppDbContext _context;
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

        // --- User Management ---

        // جعل هذه الدوال خاصة (private) لأنها دوال مساعدة لـ CreateUserAsync
        private async Task<ServiceResult<UserDetailDto>> CreateStudentAsync(CreateStudentByAdminDto studentDto)
        {
            _logger.LogInformation("Attempting to create student with email {Email}", studentDto.Email);
            var existingUser = await _userManager.FindByEmailAsync(studentDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Student creation failed: Email {Email} already exists.", studentDto.Email);
                return ServiceResult<UserDetailDto>.Failed($"Email '{studentDto.Email}' is already in use.");
            }

            var studentUser = new User
            {
                UserName = studentDto.UserName ?? studentDto.Email,
                Email = studentDto.Email,
                FullName = studentDto.FullName,
                EmailConfirmed = studentDto.EmailConfirmed
            };

            var identityResult = await _userManager.CreateAsync(studentUser, studentDto.Password);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToList();
                _logger.LogWarning("Student User creation failed for email {Email}. Errors: {Errors}", studentDto.Email, string.Join(", ", errors));
                return ServiceResult<UserDetailDto>.Failed(errors);
            }

            await _userManager.AddToRoleAsync(studentUser, "Student");

            var studentProfile = new Student { UserId = studentUser.Id };

            try
            {
                await _studentRepository.AddAsync(studentProfile);
                await _studentRepository.SaveChangesAsync();

                var userDetail = _mapper.Map<UserDetailDto>(studentUser);
                userDetail.StudentProfileId = studentProfile.UserId;
                var roles = await _userManager.GetRolesAsync(studentUser);
                userDetail.Role = roles.FirstOrDefault(); // أخذ الدور الأول من القائمة
                _logger.LogInformation("Successfully created student with ID {StudentId} and UserID {AppUserId}", studentProfile.UserId, studentUser.Id);
                return ServiceResult<UserDetailDto>.Succeeded(userDetail, "Student created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student profile for UserID {AppUserId} after user creation.", studentUser.Id);
                await _userManager.DeleteAsync(studentUser);
                return ServiceResult<UserDetailDto>.Failed("An error occurred while creating the student profile. The user account was rolled back.");
            }
        }

        private async Task<ServiceResult<UserDetailDto>> CreateProfessorAsync(CreateProfessorByAdminDto professorDto)
        {
            _logger.LogInformation("Attempting to create professor with email {Email}", professorDto.Email);
            var existingUser = await _userManager.FindByEmailAsync(professorDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Professor creation failed: Email {Email} already exists.", professorDto.Email);
                return ServiceResult<UserDetailDto>.Failed($"Email '{professorDto.Email}' is already in use.");
            }

            var professorUser = new User
            {
                UserName = professorDto.UserName ?? professorDto.Email,
                Email = professorDto.Email,
                FullName = professorDto.FullName,
                EmailConfirmed = professorDto.EmailConfirmed
            };

            var identityResult = await _userManager.CreateAsync(professorUser, professorDto.Password);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToList();
                _logger.LogWarning("Professor User creation failed for email {Email}. Errors: {Errors}", professorDto.Email, string.Join(", ", errors));
                return ServiceResult<UserDetailDto>.Failed(errors);
            }

            await _userManager.AddToRoleAsync(professorUser, "Professor");

            var professorProfile = new Professor
            {
                UserId = professorUser.Id,
                Specialization = professorDto.Specialization
            };

            try
            {
                await _professorRepository.AddAsync(professorProfile);
                await _professorRepository.SaveChangesAsync();

                var userDetail = _mapper.Map<UserDetailDto>(professorUser);
                userDetail.ProfessorProfileId = professorProfile.UserId;
                userDetail.Specialization = professorProfile.Specialization;
                var roles = await _userManager.GetRolesAsync(professorUser);
                userDetail.Role = roles.FirstOrDefault(); // أخذ الدور الأول من القائمة
                _logger.LogInformation("Successfully created professor with ID {ProfessorId} and UserID {AppUserId}", professorProfile.UserId, professorUser.Id);
                return ServiceResult<UserDetailDto>.Succeeded(userDetail, "Professor created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating professor profile for UserID {AppUserId} after user creation.", professorUser.Id);
                await _userManager.DeleteAsync(professorUser);
                return ServiceResult<UserDetailDto>.Failed("An error occurred while creating the professor profile. The user account was rolled back.");
            }
        }

        public async Task<ServiceResult<UserDetailDto>> CreateUserAsync(RegisterDto registrationData)
        {
            if (registrationData.RoleName == "Student")
            {
                var dto = new CreateStudentByAdminDto
                {
                    Email = registrationData.Email,
                    Password = registrationData.Password,
                    FullName = registrationData.FullName,
                    EmailConfirmed = true,
                };
                return await CreateStudentAsync(dto);
            }
            else if (registrationData.RoleName == "Professor")
            {
                var dto = new CreateProfessorByAdminDto
                {
                    Email = registrationData.Email,
                    Password = registrationData.Password,
                    FullName = registrationData.FullName,
                    EmailConfirmed = true,
                    // إذا كانت Specialization تساوي null، استخدم قيمة افتراضية مثل "Not specified"
                    Specialization = registrationData.Specialization ?? "Not specified"
                };
                return await CreateProfessorAsync(dto);
            }

            return ServiceResult<UserDetailDto>.Failed("Unsupported role provided.");
        }

        public async Task<ServiceResult<UserDetailDto>> GetUserDetailsByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return ServiceResult<UserDetailDto>.Failed("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault(); // تعريف متغير جديد للدور

            var dto = _mapper.Map<UserDetailDto>(user);
            dto.Role = userRole; // إسناد الدور الصحيح

            if (roles.Contains("Student")) // استخدم القائمة roles للتحقق
            {
                var student = await _studentRepository.GetByUserIdAsync(user.Id);
                if (student != null)
                {
                    dto.StudentProfileId = student.UserId;
                }
            }
            else if (roles.Contains("Professor")) // استخدم القائمة roles للتحقق
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
            var usersQuery = _userManager.Users;
            List<User> users;

            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = (await _userManager.GetUsersInRoleAsync(roleFilter)).ToList();
            }
            else
            {
                users = await usersQuery.ToListAsync();
            }

            var userDtos = new List<UserDetailDto>();
            foreach (var user in users)
            {
                var dto = _mapper.Map<UserDetailDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                dto.Role = roles.FirstOrDefault(); // أخذ الدور الأول من القائمة

                if (dto.Role.Contains("Student"))
                {
                    var student = await _studentRepository.GetByUserIdAsync(user.Id);
                    if (student != null)
                    {
                        dto.StudentProfileId = student.UserId;
                    }
                }
                else if (dto.Role.Contains("Professor"))
                {
                    var prof = await _professorRepository.GetByUserIdAsync(user.Id);
                    if (prof != null)
                    {
                        dto.ProfessorProfileId = prof.UserId;
                        dto.Specialization = prof.Specialization;
                    }
                }
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
            user.UserName = updateDto.Email ?? user.UserName; // Best practice to keep UserName and Email in sync

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors.Select(e => e.Description).ToList();
                return ServiceResult<UserDetailDto>.Failed(errors);
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

            // Return the updated details
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

            // The related Student/Professor profile will be deleted by the database's cascade delete constraint.
            // If you don't have cascade delete, you must delete them manually first as in your original code.

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

        // <-- تصحيح: إضافة `ServiceResult` وتغليف المنطق
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
            var course = _mapper.Map<Course>(courseDto);
            course.TotalHours = course.PracticalHours + course.TheoreticalHours;

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();

            var resultDto = _mapper.Map<CourseDetailDto>(course);
            return ServiceResult<CourseDetailDto>.Succeeded(resultDto, "Course created successfully.");
        }

        // <-- إضافة: تنفيذ الدالة الناقصة
        public async Task<ServiceResult<CourseDetailDto>> GetCourseByIdAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return ServiceResult<CourseDetailDto>.Failed("Course not found.");
            }
            var dto = _mapper.Map<CourseDetailDto>(course);
            return ServiceResult<CourseDetailDto>.Succeeded(dto);
        }

        // <-- تصحيح: تعديل الدالة لتتطابق مع الواجهة
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
                return ServiceResult.Failed("Course not found.");

            _mapper.Map(courseDto, course);
            course.TotalHours = course.PracticalHours + course.TheoreticalHours;

           
            //  await لأنها متزامنة
            _courseRepository.Update(course);
            // -----------------

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