// In: kalamon_University/Interfaces/IWarningRepository.cs

using kalamon_University.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kalamon_University.Interfaces
{
    public interface IWarningRepository
    {
        // إضافة إنذار جديد
        Task AddAsync(Warning warning);

        // جلب كل الإنذارات لطالب معين
        Task<IEnumerable<Warning>> GetWarningsForStudentAsync(Guid studentId);

        // حفظ التغييرات
        Task SaveChangesAsync();
        //دالة للتحقق من وجود إنذار غياب سابق
        Task<bool> HasAbsenceWarningForCourseAsync(Guid studentId, int courseId);
    }
}