// In: kalamon_University/Interfaces/IAttendanceService.cs

using kalamon_University.DTOs.Attendance;
using kalamon_University.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kalamon_University.Interfaces
{
    /// <summary>
    /// تدير منطق العمل المتعلق بالحضور والغياب، بما في ذلك تسجيل الحضور وإصدار الإنذارات.
    /// </summary>
    public interface IAttendanceService
    {
        /// <summary>
        /// يسجل حضور أو غياب طالب في جلسة معينة لمقرر معين.
        /// يقوم بالتحقق تلقائيًا من حد الغياب وإصدار إنذار إذا لزم الأمر.
        /// </summary>
        /// <param name="recordDto">بيانات الحضور المراد تسجيلها.</param>
        /// <returns>نتيجة العملية مع بيانات سجل الحضور الذي تم إنشاؤه.</returns>
        Task<ServiceResult<AttendanceRecordDto>> RecordAttendanceAsync(RecordAttendanceDto recordDto);

        /// <summary>
        /// يجلب سجلات الحضور والغياب لطالب معين في مقرر محدد.
        /// </summary>
        /// <param name="studentId">معرف الطالب.</param>
        /// <param name="courseId">معرف المقرر.</param>
        /// <returns>قائمة بسجلات الحضور للطالب.</returns>
        Task<ServiceResult<IEnumerable<AttendanceRecordDto>>> GetAttendanceForStudentInCourseAsync(Guid studentId, int courseId);

        // يمكنك إضافة دوال أخرى في المستقبل هنا، مثل:
        // Task<ServiceResult> BulkRecordAttendanceAsync(IEnumerable<RecordAttendanceDto> records);
        // Task<ServiceResult<AttendanceSummaryDto>> GetAttendanceSummaryForStudentAsync(Guid studentId, int courseId);
    }
}