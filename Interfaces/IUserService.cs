﻿namespace kalamon_University.Interfaces
using System.Collections.Generic;
using System.Threading.Tasks;
using kalamon_University.Models.Entities;
using kalamon_University.Enums;


public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task<User> RegisterUserAsync(RegisterUserDto dto);
    Task<User?> AuthenticateAsync(LoginDto dto);
}
