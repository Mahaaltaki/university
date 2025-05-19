namespace kalamon_University.Interfaces
{
    using System.Threading.Tasks;
    using kalamon_University.Models.Entities;

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }

}
