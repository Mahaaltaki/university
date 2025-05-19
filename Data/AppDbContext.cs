using Microsoft.EntityFrameworkCore;
using kalamon_University.Models.Entities;
namespace Kalanon_University.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Warning> Warnings { get; set; }
        public DbSet<User> Users { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // مفاتيح أساسية
            modelBuilder.Entity<Student>()
                .HasKey(s => s.UserID);

            modelBuilder.Entity<Professor>()
                .HasKey(p => p.UserID);

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentID, sc.CourseID });

            // تحويل Enum Role إلى نص
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
            //تحويل Enum Attendance إلى نص
            modelBuilder.Entity<Attendance>()
           .Property(a => a.Status)
           .HasConversion<string>();


            // علاقة 1:1 بين User و Student
            modelBuilder.Entity<User>()
                .HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserID);

            // علاقة 1:1 بين User و Professor
            modelBuilder.Entity<User>()
                .HasOne(u => u.Professor)
                .WithOne(p => p.User)
                .HasForeignKey<Professor>(p => p.UserID);


            base.OnModelCreating(modelBuilder);
        }
    }
}
