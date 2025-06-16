using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using kalamon_University.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=UniversityDB;Trusted_Connection=True;");

        return new AppDbContext(optionsBuilder.Options);
    }

}
