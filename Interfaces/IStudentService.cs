using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kalamon_University.Models.Entities;

namespace kalamon_University.Interfaces
{
    /// <summary>
    /// واجهة لخدمات إدارة الطلاب، توفر عمليات الإضافة والقراءة والتعديل والحذف.
    /// </summary>
    public interface IStudentService
    {
        /// <summary>
        /// جلب كافة الطلاب من قاعدة البيانات.
        /// </summary>
        /// <returns>قائمة تحتوي على جميع الطلاب.</returns>
        Task<IEnumerable<Student>> GetAllAsync();

        /// <summary>
        /// البحث عن طالب محدد باستخدام المعرف الخاص به (UserId).
        /// </summary>
        /// <param name="studentId">المعرف الفريد (Guid) للطالب.</param>
        /// <returns>كائن الطالب إذا تم العثور عليه، وإلا سيعيد null.</returns>
        Task<Student?> GetByIdAsync(Guid studentId);

        /// <summary>
        /// إضافة طالب جديد إلى قاعدة البيانات.
        /// </summary>
        /// <param name="student">كائن الطالب الذي يحتوي على البيانات الجديدة.</param>
        /// <returns>كائن الطالب بعد إضافته وحفظه في قاعدة البيانات.</returns>
        Task<Student> AddAsync(Student student);

        /// <summary>
        /// تعديل بيانات طالب موجود بالفعل في قاعدة البيانات.
        /// </summary>
        /// <param name="student">كائن الطالب الذي يحتوي على البيانات المحدثة.</param>
        /// <returns>Task يشير إلى اكتمال العملية.</returns>
        Task UpdateAsync(Student student);

        /// <summary>
        /// حذف طالب من قاعدة البيانات بناءً على المعرف الخاص به.
        /// </summary>
        /// <param name="studentId">المعرف الفريد (Guid) للطالب المراد حذفه.</param>
        /// <returns>True إذا تمت عملية الحذف بنجاح، و False إذا لم يتم العثور على الطالب.</returns>
        Task<bool> DeleteAsync(Guid studentId);
    }
}