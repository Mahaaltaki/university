﻿namespace kalamon_University.DTOs.Auth
public class RegisterUserDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required Role Role { get; set; } 

    public string? Specialization { get; set; }
}
