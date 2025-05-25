namespace kalamon_University.Interfaces.ICourseService.cs
using System.Collections.Generic;
using System;
using kalamon_University.Core.Models;
using kalamon_University.DTOs.Common
using System.Threading.Tasks;
using kalamon_University.DTOs.Course; // Make sure to use the correct namespace for DTOs

public interface ICourseService
{
    // --- Admin Operations ---
    Task<ServiceResult<CourseDto>> CreateCourseAsync(CreateCourseDto courseDto);
    Task<ServiceResult> UpdateCourseAsync(int courseId, UpdateCourseDto courseDto);
    Task<ServiceResult> DeleteCourseAsync(int courseId);
    Task<ServiceResult<IEnumerable<StudentInCourseDto>>> GetStudentsInCourseAsync(int courseId);
    Task<ServiceResult> EnrollStudentInCourseAsync(EnrollStudentInCourseDto enrollmentDto);
    Task<ServiceResult> RemoveStudentFromCourseAsync(int courseId, Guid studentId);

    // --- Common Operations (Admin, Professor, Student with different filtering) ---
    Task<ServiceResult<CourseDto>> GetCourseByIdAsync(int courseId);
    Task<ServiceResult<IEnumerable<CourseDto>>> GetAllCoursesAsync(string? searchQuery = null); // All courses for admin/student to browse

    // --- Professor Specific Operations ---
    Task<ServiceResult<IEnumerable<CourseDto>>> GetCoursesByProfessorAsync(Guid ProfessorId);

    // --- Student Specific Operations ---
    Task<ServiceResult<IEnumerable<CourseDto>>> GetCoursesByStudentAsync(Guid studentId); // Courses student is enrolled in
}