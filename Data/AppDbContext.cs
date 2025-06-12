using Microsoft.AspNetCore.Identity; // <--- تصحيح
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using kalamon_University.Models.Entities;
using System; // <--- إضافة لاستخدام Guid

namespace kalamon_University.Data
{
    // يرث من IdentityDbContext مع تحديد أنواع User, Role, و Key
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        // DbSets للكيانات غير التابعة لـ Identity مباشرة
        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Attendance> Attendances { get; set; } 
        public DbSet<Warning> Warnings { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // مهم جدًا لـ Identity

            // --- تكوين العلاقات والمفاتيح ---

            // Enrollment (Many-to-Many بين Student و Course)
            builder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.CourseId });

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Enrollments) // افترض اسم خاصية التصفح هو Enrollments
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments) // افترض اسم خاصية التصفح هو Enrollments
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Course and Professor (One-to-Many)
            builder.Entity<Course>(entity =>
            {
                entity.HasOne(c => c.Professor)
                    .WithMany(p => p.TaughtCourses) // خاصية الملاحة في Professor
                    .HasForeignKey(c => c.ProfessorId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

            });



            // User and Student (One-to-One) - افترض Student.UserId هو PK و FK
            // إذا كان لـ User خاصية Student? StudentProfile
            builder.Entity<User>()
                .HasOne(u => u.StudentProfile) //  اسم الخاصية في User هو StudentProfile
                .WithOne(s => s.User)          //  اسم الخاصية العكسية في Student هو User
                .HasForeignKey<Student>(s => s.UserId); // UserId في Student هو FK لـ User.Id

            // User and Professor (One-to-One) - Professor.UserId هو PK و FK
            // إذا كان لـ User خاصية Professor? ProfessorProfile
            builder.Entity<User>()
                .HasOne(u => u.ProfessorProfile) //  اسم الخاصية في User هو ProfessorProfile
                .WithOne(p => p.User)           //  اسم الخاصية العكسية في Professor هو User
                .HasForeignKey<Professor>(p => p.UserId); // UserId في Professor هو FK لـ User.Id


            // Student Configuration (Index)
            builder.Entity<Student>(entity =>
            {
                entity.HasIndex(s => s.UserId).IsUnique();
            });

            builder.Entity<Warning>()
             .Property(w => w.Type)
              .HasConversion<string>();

            // Attendance (افترضنا الكيان اسمه Attendance)
            builder.Entity<Attendance>(entity =>
            {
                entity.HasOne(a => a.Student) //  Attendance.Student
                    .WithMany(s => s.Attendances) //  Student.Attendances
                    .HasForeignKey(a => a.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Course)
                    .WithMany(c => c.Attendances) //  Course.Attendances
                    .HasForeignKey(a => a.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Warning
            builder.Entity<Warning>(entity =>
            {
                entity.HasOne(w => w.Student) //  Warning.Student
                    .WithMany(s => s.Warnings) //  Student.WarningsReceived (أو Warnings)
                    .HasForeignKey(w => w.StudentId)    //  Warning.StudentId هو FK لـ Student.Id
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(w => w.Course)
                    .WithMany(c => c.Warnings) //  Course.Warnings
                    .HasForeignKey(w => w.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

              
            });

            // Notification
            builder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.TargetUser)
                    .WithMany(u => u.Notifications) // تأكد أن User فيه `ICollection<Notification> Notifications`
                    .HasForeignKey(n => n.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });



            // --- تهيئة بيانات أولية (Seeding Data) ---
            var adminRoleId = Guid.NewGuid();
            var professorRoleId = Guid.NewGuid();
            var studentRoleId = Guid.NewGuid();

            builder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid> { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<Guid> { Id = professorRoleId, Name = "Professor", NormalizedName = "PROFESSOR" },
                new IdentityRole<Guid> { Id = studentRoleId, Name = "Student", NormalizedName = "STUDENT" }
            );

           
        }
    }
}