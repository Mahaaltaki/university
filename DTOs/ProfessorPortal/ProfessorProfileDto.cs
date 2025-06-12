
using System;

namespace kalamon_University.DTOs.ProfessorPortal
{
    public record ProfessorProfileDto
    {
        // From User entity
        public Guid UserId { get; init; } // هذا هو User.Id وهو أيضًا Professor.UserId (PK)
        public string FullName { get; init; } // User.Name
        public string Email { get; init; }    // User.Email
        public string? UserName { get; init; } // User.UserName
        public bool EmailConfirmed { get; init; } // User.EmailConfirmed

        // From Professor entity
        public string Specialization { get; init; } // Professor.Specialization

    }
}