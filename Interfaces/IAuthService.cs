namespace kalamon_University.Interfaces
using System.Threading.Tasks;


public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto model, string role);
    Task<AuthResultDto> LoginAsync(LoginDto model);
    // Task<AuthResultDto> RefreshTokenAsync(string token);
}