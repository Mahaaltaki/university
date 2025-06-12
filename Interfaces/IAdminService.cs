// kalamon_University.Interfaces.IAdminService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kalamon_University.DTOs.Admin;
using kalamon_University.DTOs.Auth;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Course; // 

namespace kalamon_University.Interfaces
{
    public interface IAdminService
    {
        // --- User Management (Students & Professors) ---
        Task<ServiceResult<UserDetailDto>> CreateUserAsync(RegisterDto registrationData);
        Task<ServiceResult<UserDetailDto>> GetUserDetailsByIdAsync(Guid userId);
        Task<ServiceResult<IEnumerable<UserDetailDto>>> GetAllUsersAsync(string? roleFilter = null);
        Task<ServiceResult<UserDetailDto>> UpdateUserAsync(Guid userId, UpdateUserDto dto);
        Task<ServiceResult> DeleteUserAsync(Guid userId);

        // --- Role & Account Management ---
        Task<ServiceResult> AssignRoleToUserAsync(Guid userId, string roleName);
        
        
        Task<ServiceResult> ConfirmUserEmailByAdminAsync(Guid userId, bool confirmedStatus);

        // --- Course Management by Admin ---
        Task<ServiceResult<CourseDetailDto>> CreateCourseAsync(CreateCourseDto courseDto);
        Task<ServiceResult<CourseDetailDto>> GetCourseByIdAsync(int courseId);
        Task<ServiceResult<IEnumerable<CourseDetailDto>>> GetAllCoursesAsync(bool includeProfessorDetails = false);
        Task<ServiceResult> UpdateCourseAsync(int courseId, UpdateCourseDto courseDto);
        Task<ServiceResult> DeleteCourseAsync(int courseId);
        Task<ServiceResult> AssignProfessorToCourseAsync(int courseId, Guid professorUserId);
        Task<ServiceResult> UnassignProfessorFromCourseAsync(int courseId);
        

    }
}