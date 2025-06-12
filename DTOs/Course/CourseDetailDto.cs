// kalamon_University.DTOs.Course.CourseDetailDto.cs
using System;

namespace kalamon_University.DTOs.Course
{
    public class CourseDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string? Description { get; set; } = null;
        public int PracticalHours { get; set; }
        public int TheoreticalHours { get; set; }
        public int TotalHours { get; set; }
        public int MaxAbsenceLimit { get; set; }

        public Guid? ProfessorId { get; set; }

        public string ProfessorName { get; set; }
        
     
    }
}