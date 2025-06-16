using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using kalamon_University.Data; // افترض أن DbContext موجود في هذا المسار
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;

namespace kalamon_University.Services
{
    /// <summary>
    /// فئة الخدمة التي تنفذ الواجهة IStudentService وتوفر منطق العمل لإدارة الطلاب.
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context; // سياق قاعدة البيانات
        private readonly ILogger<StudentService> _logger; // خدمة تسجيل الأخطاء

        /// <summary>
        /// المنشئ (Constructor) الذي يقوم بحقن التبعيات المطلوبة.
        /// </summary>
        /// <param name="context">سياق قاعدة البيانات للتفاعل معها.</param>
        /// <param name="logger">خدمة لتسجيل المعلومات والأخطاء.</param>
        public StudentService(AppDbContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            try
            {
                // نستخدم Include لجلب البيانات المرتبطة من جدول Users (Eager Loading)
                // هذا ضروري لعرض معلومات الطالب مثل الاسم، والتي توجد في كيان User.
                return await _context.Students
                                     .Include(s => s.User)
                                     .AsNoTracking() // يحسن الأداء لعمليات القراءة فقط
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب قائمة جميع الطلاب.");
                throw; // إعادة رمي الخطأ ليتم التعامل معه في طبقة أعلى (مثل Controller)
            }
        }

        /// <inheritdoc />
        public async Task<Student?> GetByIdAsync(Guid studentId)
        {
            try
            {
                return await _context.Students
                                     .Include(s => s.User)
                                     .FirstOrDefaultAsync(s => s.UserId == studentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء البحث عن طالب بالمعرف: {StudentId}", studentId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Student> AddAsync(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }

            try
            {
                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات
                return student; // إعادة الطالب بعد إضافته
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إضافة طالب جديد.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }

            // نبلغ EF Core بأن حالة هذا الكيان هي "معدل"
            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            // هذا الخطأ يحدث إذا حاول مستخدمان تعديل نفس السجل في نفس الوقت
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "حدث خطأ تضارب أثناء تحديث بيانات الطالب ذو المعرف: {StudentId}", student.UserId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث بيانات الطالب ذو المعرف: {StudentId}", student.UserId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid studentId)
        {
            try
            {
                var studentToDelete = await _context.Students.FindAsync(studentId);

                if (studentToDelete == null)
                {
                    _logger.LogWarning("محاولة حذف طالب غير موجود بالمعرف: {StudentId}", studentId);
                    return false; // الطالب غير موجود
                }

                _context.Students.Remove(studentToDelete);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف الطالب ذو المعرف: {StudentId} بنجاح.", studentId);
                return true; // تم الحذف بنجاح
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف الطالب ذو المعرف: {StudentId}", studentId);
                throw;
            }
        }
    }
}