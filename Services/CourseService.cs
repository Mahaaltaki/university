using kalamon_University.Data; // استبدل هذا بالـ namespace الخاص بـ DbContext لديك
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Course;
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kalamon_University.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context; // اسم الـ DbContext الخاص بك

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        // --- Admin Operations ---

        public async Task<ServiceResult<CourseDetailDto>> CreateCourseAsync(CreateCourseDto courseDto)
        {
            var newCourse = new Course
            {
                Name = courseDto.Name,
                ProfessorId = courseDto.ProfessorId,
                PracticalHours = courseDto.PracticalHours,
                TheoreticalHours = courseDto.TheoreticalHours,
                TotalHours = courseDto.TotalHours,
                MaxAbsenceLimit = courseDto.MaxAbsenceLimit
            };

            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            // قم بإعادة جلب الكورس مع بيانات البروفيسور لعرضها في النتيجة
            var resultDto = await GetCourseByIdAsync(newCourse.Id);
            return resultDto;
        }

        public async Task<ServiceResult> UpdateCourseAsync(int courseId, UpdateCourseDto courseDto)
        {
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null)
            {
                return ServiceResult.Failed("Course not found.");
            }

            // تحديث الخصائص
            course.Name = courseDto.Name;
            course.ProfessorId = courseDto.ProfessorId;
            course.PracticalHours = courseDto.PracticalHours;
            course.TheoreticalHours = courseDto.TheoreticalHours;
            course.TotalHours = courseDto.TotalHours;
            course.MaxAbsenceLimit = courseDto.MaxAbsenceLimit;

            await _context.SaveChangesAsync();
            return ServiceResult.Succeeded("Course updated successfully.");
        }

        public async Task<ServiceResult> DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null)
            {
                return ServiceResult.Failed("Course not found.");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return ServiceResult.Succeeded("Course deleted successfully.");
        }

        public async Task<ServiceResult<IEnumerable<StudentInCourseDto>>> GetStudentsInCourseAsync(int courseId)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
            {
                return ServiceResult<IEnumerable<StudentInCourseDto>>.Failed("Course not found.");
            }

            var students = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student.User) // تضمين بيانات الطالب ثم المستخدم المرتبط به
                .Select(e => new StudentInCourseDto
                {
                    StudentId = e.StudentId,
                    // افترض أن لديك FullName أو FirstName و LastName في كائن User
                    FullName = e.Student.User.FullName, // أو e.Student.User.FirstName + " " + e.Student.User.LastName
                    Email = e.Student.User.Email,
                    EnrollmentDate = e.EnrollmentDate
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<StudentInCourseDto>>.Succeeded(students);
        }

        public async Task<ServiceResult> EnrollStudentInCourseAsync(EnrollStudentInCourseDto enrollmentDto)
        {
            var studentExists = await _context.Users.AnyAsync(u => u.Id == enrollmentDto.StudentId);
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == enrollmentDto.CourseId);

            if (!studentExists || !courseExists)
            {
                return ServiceResult.Failed("Student or Course not found.");
            }

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == enrollmentDto.StudentId && e.CourseId == enrollmentDto.CourseId);

            if (alreadyEnrolled)
            {
                return ServiceResult.Failed("Student is already enrolled in this course.");
            }

            var newEnrollment = new Enrollment
            {
                StudentId = enrollmentDto.StudentId,
                CourseId = enrollmentDto.CourseId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(newEnrollment);
            await _context.SaveChangesAsync();

            return ServiceResult.Succeeded("Student enrolled successfully.");
        }

        public async Task<ServiceResult> RemoveStudentFromCourseAsync(int courseId, Guid studentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment == null)
            {
                return ServiceResult.Failed("Enrollment record not found.");
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return ServiceResult.Succeeded("Student removed from course successfully.");
        }


        // --- Common Operations ---

        public async Task<ServiceResult<CourseDetailDto>> GetCourseByIdAsync(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Professor.User) // تضمين البروفيسور ثم المستخدم المرتبط به للحصول على الاسم
                .Where(c => c.Id == courseId)
                .Select(c => new CourseDetailDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProfessorId = c.ProfessorId,
                    // عرض اسم البروفيسور إذا كان موجودًا
                    ProfessorName = c.Professor != null ? c.Professor.User.FullName : "N/A",
                    PracticalHours = c.PracticalHours,
                    TheoreticalHours = c.TheoreticalHours,
                    TotalHours = c.TotalHours,
                    MaxAbsenceLimit = c.MaxAbsenceLimit
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return ServiceResult<CourseDetailDto>.Failed("Course not found.");
            }

            return ServiceResult<CourseDetailDto>.Succeeded(course);
        }

        public async Task<ServiceResult<IEnumerable<CourseDetailDto>>> GetAllCoursesAsync(string? searchQuery = null)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(c => c.Name.Contains(searchQuery));
            }

            var courses = await query
                .Include(c => c.Professor.User)
                .Select(c => new CourseDetailDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProfessorId = c.ProfessorId,
                    ProfessorName = c.Professor != null ? c.Professor.User.FullName : "N/A",
                    PracticalHours = c.PracticalHours,
                    TheoreticalHours = c.TheoreticalHours,
                    TotalHours = c.TotalHours,
                    MaxAbsenceLimit = c.MaxAbsenceLimit
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<CourseDetailDto>>.Succeeded(courses);
        }


        // --- Professor Specific Operations ---

        public async Task<ServiceResult<IEnumerable<CourseDetailDto>>> GetCoursesByProfessorAsync(Guid professorId)
        {
            var courses = await _context.Courses
                .Where(c => c.ProfessorId == professorId)
                .Include(c => c.Professor.User)
                .Select(c => new CourseDetailDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProfessorId = c.ProfessorId,
                    ProfessorName = c.Professor.User.FullName,
                    PracticalHours = c.PracticalHours,
                    TheoreticalHours = c.TheoreticalHours,
                    TotalHours = c.TotalHours,
                    MaxAbsenceLimit = c.MaxAbsenceLimit
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<CourseDetailDto>>.Succeeded(courses);
        }

        // --- Student Specific Operations ---

        public async Task<ServiceResult<IEnumerable<CourseDetailDto>>> GetCoursesByStudentAsync(Guid studentId)
        {
            var courses = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course.Professor.User) // تضمين الكورس ثم البروفيسور ثم المستخدم
                .Select(e => new CourseDetailDto
                {
                    Id = e.Course.Id,
                    Name = e.Course.Name,
                    ProfessorId = e.Course.ProfessorId,
                    ProfessorName = e.Course.Professor != null ? e.Course.Professor.User.FullName : "N/A",
                    PracticalHours = e.Course.PracticalHours,
                    TheoreticalHours = e.Course.TheoreticalHours,
                    TotalHours = e.Course.TotalHours,
                    MaxAbsenceLimit = e.Course.MaxAbsenceLimit
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<CourseDetailDto>>.Succeeded(courses);
        }
    }
}