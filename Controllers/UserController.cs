
using kalamon_University.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace kalamon_University.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public UserController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    //  1. تسجيل الدخول
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userService.GetUserByEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
        {
            return Unauthorized("Invalid email or password");
        }

        // توليد التوكن
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }


    //  2. إضافة مستخدم جديد
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        await _userService.AddUserAsync(user);
        return Ok(user);
    }

    //  3. تحديث معلومات مستخدم
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
    {
        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
            return NotFound();

        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;
        existingUser.Password = updatedUser.Password;
        existingUser.Role = updatedUser.Role;

        await _userService.UpdateUserAsync(existingUser);

        return Ok(existingUser);
    }

    //  4. حذف مستخدم
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok();
    }

    //  5. عرض جميع المستخدمين
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    //  6. عرض مستخدم مفرد
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }
}
