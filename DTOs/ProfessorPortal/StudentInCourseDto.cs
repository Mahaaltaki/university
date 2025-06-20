
using System;

namespace kalamon_University.DTOs.ProfessorPortal
{
    public record StudentInCourseDto
    {
        public Guid StudentUserId { get; init; } // User.Id of the student
        public Guid StudentEntityId { get; init; } // Student.Id (PK of Students table)
        public string StudentFullName { get; init; }
        public string StudentIdEmail { get; init; } // The university ID number of the student
    }
}