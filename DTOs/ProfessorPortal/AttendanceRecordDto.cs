namespace kalamon_University.DTOs.ProfessorPortal
{//عرض سجل حضور طالب معين
    public class AttendanceRecordDto
    {
        public int AttendanceId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public bool IsPresent { get; set; }
        public string? Notes { get; set; }
    }
}
