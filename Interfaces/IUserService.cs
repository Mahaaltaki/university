using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kalamon_University.Models.Entities; // لـ User
using kalamon_University.DTOs.Admin;
using kalamon_University.DTOs.Common; // لـ ServiceResult إذا كنت ستستخدمه هنا
using kalamon_University.DTOs.Auth;
namespace kalamon_University.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        Task<User?> AddUserAsync(RegisterDto dto);

        /// <summary>
        /// Gets a user by their ID, including their roles and profile information (Student/Professor).
        /// </summary>
        Task<UserDetailDto?> GetUserDetailedByIdAsync(Guid userId);

        /// <summary>
        /// Gets a user by their email.
        /// </summary>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        Task<User?> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// Updates non-critical profile information for a user.
        /// </summary>
        Task<ServiceResult> UpdateUserProfileAsync(Guid userId, UpdateUserDto dto);

        /// <summary>
        /// Gets all users (potentially with pagination and filtering).
        /// </summary>
        Task<IEnumerable<UserDetailDto>> GetAllUsersAsync();
    }
}
