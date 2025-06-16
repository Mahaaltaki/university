using System;
using System.Collections.Generic;

namespace kalamon_University.DTOs.Admin
{
    public class UserDetailDto
    {
        public Guid Id { get; set; } // User.Id (PK from AspNetUsers)
        public string Email { get; set; }
        public string Name { get; set; } // User.Name (Full Name)
        public string FullName { get; set; } // User.UserName (Login Name)
        public string Role { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        // Student specific info
        public Guid? StudentProfileId { get; set; } // PK of the Student entity
        

        // Professor specific info
        public Guid? ProfessorProfileId { get; set; } // PK of the Professor entity
       
        public string? Specialization { get; set; }
    }
}