using Microsoft.EntityFrameworkCore;
using kalamon_University.Models.Entities; // حسب مكان الكيانات لديك

namespace kalamon_University.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        // أضف بقية DbSets هنا
    }
}
