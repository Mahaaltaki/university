namespace kalamon_University.Models.Enums
{
    public static class RoleExtensions
    {
        public static string ToRoleName(this Role role)
        {
            return role.ToString();
        }
    }
}
