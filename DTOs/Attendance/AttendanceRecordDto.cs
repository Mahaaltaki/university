// In: DTOs/Attendance/AttendanceRecordDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Attendance
{
    public class AttendanceRecordDto
    {
        public int Id { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        public string? Notes { get; set; }
    }
}