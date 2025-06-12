using kalamon_University.DTOs.Admin;

namespace kalamon_University.DTOs.Auth
{
    public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Token { get; set; }
        public UserDetailDto User { get; set; }
    }
}
