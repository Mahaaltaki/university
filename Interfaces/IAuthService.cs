// kalamon_University.Interfaces.IAuthService.cs
using System.Threading.Tasks;
using kalamon_University.DTOs.Auth; // б№ RegisterDto, LoginDto, AuthResultDto

namespace kalamon_University.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user (Student or Professor) by an admin or self-registration.
        /// Creates the IdentityUser and the associated Student/Professor profile.
        /// </summary>
        Task<AuthResultDto> RegisterAsync(RegisterDto model); // RoleName, StudentIdNumber/StaffIdNumber  яжд ÷гд RegisterDto

        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        Task<AuthResultDto> LoginAsync(LoginDto model);
    }
}