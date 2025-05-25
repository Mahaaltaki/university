using kalamon_University.Models.Entities;
using kalamon_University.DTOs;
using kalamon_University.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public AuthController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var user = await _userService.RegisterUserAsync(dto.Name, dto.Email, dto.Password, dto.Role);
        if (user == null)
            return BadRequest("Email already in use.");

        return Ok(new { message = "User registered successfully", user.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
        if (user == null)
            return Unauthorized("Invalid email or password.");

        var token = JwtHelper.GenerateToken(user, _config);
        return Ok(new { token });
    }
}
