using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Course;
using kalamon_University.Models.Entities;

namespace kalamon_University.Interfaces
{
    /// <summary>
    /// واجهة لخدمات إدارة الطلاب، توفر عمليات الإضافة والقراءة والتعديل والحذف، بالإضافة إلى العمليات الخاصة بالطالب.
    /// </summary>
    public interface IStudentService
    {
        


        #region Student-Specific Operations (العمليات الخاصة بواجهة الطالب)

        /// <summary>
        /// جلب جميع الكورسات المتاحة للتسجيل.
        /// </summary>
        /// <returns>قائمة بجميع الكورسات.</returns>
        Task<IEnumerable<CourseDetailDto>> GetAllAvailableCoursesAsync();

        /// <summary>
        /// تسجيل الطالب الحالي في كورس محدد.
        /// </summary>
        /// <param name="studentId">معرف الطالب الذي يقوم بالتسجيل.</param>
        /// <param name="courseId">معرف الكورس المراد التسجيل فيه.</param>
        /// <returns>True إذا نجحت عملية التسجيل، و False إذا كان مسجلاً بالفعل أو حدث خطأ.</returns>
        Task<ServiceResult> EnrollInCourseAsync(Guid studentId, int courseId);

        /// <summary>
        /// جلب جميع الكورسات التي سجل فيها طالب معين.
        /// </summary>
        /// <param name="studentId">معرف الطالب.</param>
        /// <returns>قائمة بالكورسات المسجل بها الطالب.</returns>
        Task<IEnumerable<Course>> GetMyEnrolledCoursesAsync(Guid studentId);

        /// <summary>
        /// جلب سجل الحضور لطالب معين في كورس محدد.
        /// </summary>
        /// <param name="studentId">معرف الطالب.</param>
        /// <param name="courseId">معرف الكورس.</param>
        /// <returns>قائمة بسجلات الحضور الخاصة بالطالب في ذلك الكورس.</returns>
        Task<IEnumerable<Attendance>> GetMyAttendanceForCourseAsync(Guid studentId, int courseId);

        #endregion
    }
}