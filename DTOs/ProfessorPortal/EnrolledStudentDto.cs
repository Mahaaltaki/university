namespace kalamon_University.DTOs.ProfessorPortal

{ //لعرض بيانات الطلاب المسجلين في كورس
    public class EnrolledStudentDto
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
