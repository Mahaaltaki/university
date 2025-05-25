using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using kalamon_University.Models.Entities;
namespace kalamon_University.Data
{
    public class AppDbContext : DbContext, IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Warning> Warnings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(builder);

            // Composite key for Enrollment
            builder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            // Relationships
            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Or Cascade if appropriate

            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany(d => d.TaughtCourses)
                .HasForeignKey(c => c.ProfessorId);

            builder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne() // Assuming one ApplicationUser maps to one Student profile
                .HasForeignKey<Student>(s => s.UserId);

            builder.Entity<Professor>()
                .HasOne(d => d.User)
                .WithOne() // Assuming one ApplicationUser maps to one Professor profile
                .HasForeignKey<Professor>(d => d.UserId);

            // Seed Roles
            builder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Professor", NormalizedName = "Professor" },
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Student", NormalizedName = "STUDENT" }
            );
        } }
