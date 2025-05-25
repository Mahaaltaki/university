using kalamon_University.DTOs.Auth;
using kalamon_University.Enums; 

public async Task<User?> RegisterUserAsync(RegisterUserDto dto)
{
    var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
    if (existingUser != null)
        return null;

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    var user = new User
    {
        Name = dto.Name,
        Email = dto.Email,
        Password = pssword, // اسم الخاصية يجب أن يكون Password أو PasswordHash حسب كلاس User
        Role = Enum.TryParse<Role>(dto.Role, out var parsedRole) ? parsedRole : Role.Student
    };

    await _userRepository.AddAsync(user);
    await _userRepository.SaveChangesAsync();

    if (user.Role == Role.Student)
    {
        var student = new Student { UserID = user.Id };
        await _studentRepository.AddAsync(student);
        await _studentRepository.SaveChangesAsync();
    }
    else if (user.Role == Role.Professor)
    {
        var professor = new Professor
        {
            UserID = user.Id,
            Specialization = dto.Specialization
        };
        await _professorRepository.AddAsync(professor);
        await _professorRepository.SaveChangesAsync();
    }

    return user;
}
