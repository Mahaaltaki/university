// In: kalamon_University/Interfaces/IAttendanceRepository.cs

using kalamon_University.Models.Entities;
using System.Threading.Tasks;

namespace kalamon_University.Interfaces
{
    // هذه الواجهة خاصة بالتعامل المباشر مع جدول الحضور في قاعدة البيانات
    public interface IAttendanceRepository
    {
        Task AddAsync(Attendance attendance);
        Task SaveChangesAsync();

        // يمكن إضافة دوال أخرى هنا في المستقبل حسب الحاجة
        // مثل التحقق من وجود سجل حضور مسبقاً
        Task<bool> HasAttendanceRecordAsync(Guid studentId, int courseId, DateTime sessionDate);
        Task<int> GetAbsenceCountForStudentAsync(Guid studentId, int courseId);
        Task<IEnumerable<Attendance>> GetAttendanceForStudentInCourseAsync(Guid studentId, int courseId);

    }
}