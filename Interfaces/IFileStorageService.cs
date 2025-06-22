// في مجلد Interfaces/IFileStorageService.cs
using System.Threading.Tasks;

namespace kalamon_University.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// حفظ ملف في مسار التخزين وإرجاع رابط للوصول إليه.
        /// </summary>
        /// <param name="fileContents">بيانات الملف.</param>
        /// <param name="fileName">اسم الملف.</param>
        /// <returns>رابط URL للوصول إلى الملف.</returns>
        Task<string> SaveFileAsync(byte[] fileContents, string fileName);

        /// <summary>
        /// جلب ملف من مسار التخزين.
        /// </summary>
        /// <param name="fileName">اسم الملف.</param>
        /// <returns>بيانات الملف ونوعه.</returns>
        Task<(byte[] fileContents, string contentType)?> GetFileAsync(string fileName);
    }
}