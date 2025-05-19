namespace kalamon_University.Models.Entities
{
    public class RegisterUserDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }

        // اختيارية فقط للبروفيسور
        public string? Specialization { get; set; }
    }

}
