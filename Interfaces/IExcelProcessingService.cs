using System.Collections.Generic;
using System.IO;
using kalamon_University.DTOs.Common; // For ServiceResult
using kalamon_University.DTOs.Excel;  // For StudentBasicInfoDto and ExcelStudentAttendanceDto

namespace kalamon_University.Interfaces;

/// واجهة تحدد العمليات المتعلقة بتوليد وقراءة ملفات Excel.
public interface IExcelProcessingService
{
    /// يقوم بتوليد قالب ملف Excel (بصيغة byte array) لورقة الحضور لكورس معين.
    /// ستحتوي الورقة على قائمة بالطلاب وأعمدة ليقوم الدكتور بتعبئة الحضور.
    /// <param name="students">قائمة بالطلاب المراد تضمينهم في الورقة (بمعلوماتهم الأساسية).</param>
    /// <param name="courseId">معرف الكورس (يُستخدم للتسمية أو السياق).</param>
    /// <returns>
    /// كائن <see cref="ServiceResult{TData}"/> يحتوي على byte array لملف Excel في حالة النجاح،
    /// أو قائمة بالأخطاء في حالة الفشل.
    /// </returns>
    ServiceResult<byte[]> GenerateAttendanceSheetTemplate(IEnumerable<DTOs.Student.StudentProfileDto> students, int courseId);

    /// يقوم بتحليل (parsing) ملف Excel الخاص بالحضور الذي تم رفعه.
    /// يقرأ البيانات من الملف ويحولها إلى قائمة من كائنات DTOs.
    /// <param name="excelStream">Stream لملف Excel المرفوع.</param>
    /// <param name="courseId">معرف الكورس لربط سجلات الحضور به.</param>
    /// <returns>
    /// كائن <see cref="ServiceResult{TData}"/> يحتوي على قائمة من <see cref="ExcelStudentAttendanceDto"/>
    /// تمثل سجلات الحضور المقروءة في حالة النجاح (قد يتضمن أخطاء جزئية إذا تم تحليل بعض الصفوف فقط)،
    /// أو قائمة بالأخطاء في حالة فشل التحليل بشكل كامل.
    /// </returns>
    ServiceResult<IEnumerable<ExcelStudentAttendanceDto>> ParseAttendanceSheet(Stream excelStream, int courseId);

}