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
    /// فئة الخدمة التي تنفذ الواجهة IProfessorService وتوفر منطق العمل لإدارة الأساتذة.
    /// </summary>
    public class ProfessorService : IProfessorService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProfessorService> _logger;

        /// <summary>
        /// المنشئ (Constructor) الذي يقوم بحقن التبعيات المطلوبة.
        /// </summary>
        /// <param name="context">سياق قاعدة البيانات للتفاعل معها.</param>
        /// <param name="logger">خدمة لتسجيل المعلومات والأخطاء.</param>
        public ProfessorService(AppDbContext context, ILogger<ProfessorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Professor>> GetAllAsync()
        {
            try
            {
                // نستخدم Include لجلب بيانات المستخدم المرتبطة بكل أستاذ (Eager Loading).
                // من الأفضل عدم جلب TaughtCourses هنا لتجنب تحميل بيانات ضخمة في قائمة العرض العام.
                return await _context.Professors
                                     .Include(p => p.User)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب قائمة جميع الأساتذة.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Professor?> GetByIdAsync(Guid professorId)
        {
            try
            {
                // عند طلب أستاذ واحد، من المفيد جلب المواد التي يدرسها أيضًا.
                return await _context.Professors
                                     .Include(p => p.User)
                                     .Include(p => p.TaughtCourses)
                                     .FirstOrDefaultAsync(p => p.UserId == professorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء البحث عن أستاذ بالمعرف: {ProfessorId}", professorId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Professor> AddAsync(Professor professor)
        {
            if (professor == null)
            {
                throw new ArgumentNullException(nameof(professor));
            }

            try
            {
                await _context.Professors.AddAsync(professor);
                await _context.SaveChangesAsync();
                return professor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إضافة أستاذ جديد.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Professor professor)
        {
            if (professor == null)
            {
                throw new ArgumentNullException(nameof(professor));
            }

            // تحقق من وجود الأستاذ قبل محاولة التحديث لتجنب أخطاء غير متوقعة.
            var existingProfessor = await _context.Professors.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == professor.UserId);
            if (existingProfessor == null)
            {
                throw new KeyNotFoundException($"لم يتم العثور على أستاذ بالمعرف: {professor.UserId}");
            }

            _context.Entry(professor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "حدث خطأ تضارب أثناء تحديث بيانات الأستاذ ذو المعرف: {ProfessorId}", professor.UserId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث بيانات الأستاذ ذو المعرف: {ProfessorId}", professor.UserId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid professorId)
        {
            try
            {
                var professorToDelete = await _context.Professors.FindAsync(professorId);

                if (professorToDelete == null)
                {
                    _logger.LogWarning("محاولة حذف أستاذ غير موجود بالمعرف: {ProfessorId}", professorId);
                    return false;
                }

                _context.Professors.Remove(professorToDelete);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف الأستاذ ذو المعرف: {ProfessorId} بنجاح.", professorId);
                return true;
            }
            catch (Exception ex)
            {
                // قد يحدث خطأ إذا كان الأستاذ مرتبطًا بمواد لا يمكن حذفها (بسبب قيود المفتاح الأجنبي)
                _logger.LogError(ex, "حدث خطأ أثناء حذف الأستاذ ذو المعرف: {ProfessorId}", professorId);
                throw;
            }
        }
    }
}