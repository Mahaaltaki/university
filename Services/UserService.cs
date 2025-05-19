using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;

namespace kalamon_University.Services
{
    public class UserService : IUserService
    {

        // حقن ال Repository عبر الكونستركتور
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IProfessorRepository _professorRepository;

        public UserService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            IProfessorRepository professorRepository)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _professorRepository = professorRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task AddUserAsync(User user)
        {
            // تشفير كلمة المرور
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);//تشفير

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // إنشاء سجل في جدول Student أو Professor حسب الدور
            if (user.Role == "student")
            {
                var student = new Student
                {
                    UserID = user.Id // Id يتم توليده بعد SaveChangesAsync
                };
                await _studentRepository.AddAsync(student);
            }
            else if (user.Role == "professor")
            {
                var professor = new Professor
                {
                    UserID = user.Id,
                    Specialization = registerDto.Specialization ?? "Unknown"
                };
                await _professorRepository.AddAsync(professor);
            }

            await _userRepository.SaveChangesAsync(); // حفظ الكل
        }



        public async Task UpdateUserAsync(User user)
        {
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                _userRepository.Delete(user);
                await _userRepository.SaveChangesAsync();
            }
        }
    }

}
