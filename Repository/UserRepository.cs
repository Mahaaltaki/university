//using kalamon_University.Models.Entities;
//using kalamon_University.Interfaces;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Security.Cryptography;
//using Microsoft.AspNetCore.Identity;
//using Kalanon_University.Data;

//public class UserService : IUserService
//{
//    private readonly IUserRepository _userRepository;
//    private readonly IStudentRepository _studentRepository;
//    private readonly IProfessorRepository _professorRepository;

//    public UserService(
//        IUserRepository userRepository,
//        IStudentRepository studentRepository,
//        IProfessorRepository professorRepository)
//    {
//        _userRepository = userRepository;
//        _studentRepository = studentRepository;
//        _professorRepository = professorRepository;
//    }

//    public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();

//    public async Task<User?> GetUserByIdAsync(int id) => await _userRepository.GetByIdAsync(id);

//    public async Task<User?> GetUserByEmailAsync(string email) => await _userRepository.GetByEmailAsync(email);

//    public async Task AddUserAsync(User user)
//    {
//        await _userRepository.AddAsync(user);
//        await _userRepository.SaveChangesAsync();
//    }

//    public async Task UpdateUserAsync(User user)
//    {
//        _userRepository.Update(user);
//        await _userRepository.SaveChangesAsync();
//    }

//    public async Task DeleteUserAsync(int id)
//    {
//        var user = await _userRepository.GetByIdAsync(id);
//        if (user != null)
//        {
//            _userRepository.Delete(user);
//            await _userRepository.SaveChangesAsync();
//        }
//    }

//    public async Task<User?> RegisterUserAsync(string name, string email, string password, Role role)
//    {
//        var existingUser = await _userRepository.GetByEmailAsync(email);
//        if (existingUser != null)
//            return null;

//        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
//        var user = new User
//        {
//            Name = name,
//            Email = email,
//            PasswordHash = hashedPassword,
//            Role = role
//        };

//        await _userRepository.AddAsync(user);
//        await _userRepository.SaveChangesAsync();

//        // إنشاء جدول خاص بالطالب أو الدكتور تلقائيًا
//        if (role == Role.Student)
//        {
//            var student = new Student { UserID = user.Id };
//            await _studentRepository.AddAsync(student);
//            await _studentRepository.SaveChangesAsync();
//        }
//        else if (role == Role.Professor)
//        {
//            var professor = new Professor { UserID = user.Id };
//            await _professorRepository.AddAsync(professor);
//            await _professorRepository.SaveChangesAsync();
//        }

//        return user;
//    }

//    public async Task<User?> AuthenticateAsync(string email, string password)
//    {
//        var user = await _userRepository.GetByEmailAsync(email);
//        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
//            return null;

//        return user;
//    }
//}
