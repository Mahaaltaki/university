﻿//kalamon_University/DTOs/ProfessorPortal/CourseTaughtDto.cs
namespace kalamon_University.DTOs.ProfessorPortal
{ //لعرض بيانات الكورس الأساسية للأستاذ
    public record ProfessorCourseDto
    {
        public int CourseId { get; init; }
        public string CourseName { get; init; }
        public int EnrolledStudentsCount { get; init; } // يتم حسابه
        public int PracticalHours { get; init; }
        public int TheoreticalHours { get; init; }
        public int TotalHours { get; init; }
        // يمكن إضافة MaxAbsenceLimit إذا أراد الأستاذ رؤيتها هنا
        // public int MaxAbsenceLimit { get; init; }
    }
}