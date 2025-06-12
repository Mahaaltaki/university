using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Attendance
{
    
    public record RecordAttendanceDto // For a Professor to record attendance for a single student in a session
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public Guid StudentId { get; set; } // Student.Id
        [Required]
        public DateTime SessionDate { get; set; }
        [Required]
        public bool IsPresent { get; set; }
        public string? Notes { get; set; }
    }

    public record StudentAttendanceStatusDto // Used for bulk attendance recording
    {
        [Required]
        public Guid StudentId { get; set; } // Student.Id
        [Required]
        public bool IsPresent { get; set; }
        public string? Notes { get; set; }
    }

    public record BulkRecordAttendanceDto // For a Professor to record attendance for multiple students in one go
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public DateTime SessionDate { get; set; }
        [Required]
        public IEnumerable<StudentAttendanceStatusDto> StudentStatuses { get; set; } = new List<StudentAttendanceStatusDto>();
    }

    public record StudentAttendanceSummaryDto // For Professor Portal to show summary
    {
        public Guid StudentId { get; set; } // Student.Id
        public string StudentName { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AttendancePercentage { get; set; }
    }

    // DTO for parsing from Excel, might also fit in DTOs/Excel/
    public record ExcelAttendanceRecordDto(
        string StudentId, // Key to find Student.Id
        DateTime SessionDate,
        bool IsPresent,
        string? Notes
    );

}