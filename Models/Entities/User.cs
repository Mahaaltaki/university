using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using kalamon_University.Models.Enums;
namespace kalamon_University.Models.Entities
{
    public class User : IdentityUser<Guid> // IdentityUser<Guid> يوفر Id من نوع Guid
    {
        // Id موروثة من IdentityUser<Guid>
        // Email موروثة من IdentityUser
        // PasswordHash موروثة من IdentityUser
        // UserName موروثة من IdentityUser (عادةً لاسم تسجيل الدخول)

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        // خاصية Email موجودة وموروثة من IdentityUser، يمكنك عمل override لها كما فعلت
        [Required]
        [EmailAddress]
        public override string Email { get; set; }

        public Role Role { get; set; } = Role.Student; // تأكد أن enum Role معرف

        // العلاقات الاختيارية
        public virtual Student? StudentProfile { get; set; }
        public virtual Professor? ProfessorProfile { get; set; }

        
        // إذا كان المستخدم طالبًا
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Warning> WarningsReceived { get; set; } = new List<Warning>();

        // كل مستخدم لديه قائمة من الإشعارات الموجهة إليه
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}