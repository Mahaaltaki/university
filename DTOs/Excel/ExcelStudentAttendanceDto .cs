using System;

using System.ComponentModel.DataAnnotations; // للاستفادة من DataAnnotations إذا احتجت للتحقق من الصحة هنا

namespace kalamon_University.DTOs.Excel; // أو UniversityApi.Api.DTOs.Attendance;

/// <summary>
/// DTO يمثل البيانات المستخرجة من صف واحد في ملف Excel الخاص بالحضور.
/// يستخدم بشكل أساسي عند تحليل (parsing) ملف Excel الذي يرفعه الدكتور.
/// </summary>
public class ExcelStudentAttendanceDto
{
    /// <summary>
    /// رقم الطالب الجامعي كما هو موجود في ملف Excel.
    /// هذا الحقل مهم لربط سجل الحضور بالطالب الصحيح في قاعدة البيانات.
    /// يجب أن يكون مطابقاً للرقم الموجود في عمود "Student ID" في ملف Excel.
    /// </summary>
    /// <remarks>
    /// في الخدمة التي تعالج هذا الـ DTO (مثل AttendanceService أو ExcelProcessingService)،
    /// ستحتاج إلى استخدام هذا الرقم للبحث عن الـ Student.Id الفعلي.
    /// </remarks>
    [Required(ErrorMessage = "Student ID Number from Excel is required.")] // يمكن إضافة تحقق هنا إذا كان الـ DTO سيُستخدم مباشرة في الـ Controller
    public string StudentId { get; set; }

    /// <summary>
    /// اسم الطالب كما هو موجود في ملف Excel (اختياري للقراءة، ولكن مفيد للتحقق).
    /// هذا الحقل قد لا يكون ضرورياً لعملية الحفظ في قاعدة البيانات إذا كان الاعتماد الكلي على StudentIdNumber،
    /// ولكنه يمكن أن يكون مفيدًا لأغراض التسجيل (logging) أو التحقق البشري.
    /// </summary>
    public string? StudentName { get; set; } // اختياري، قد يكون موجودًا في ملف Excel

    /// <summary>
    /// تاريخ جلسة المحاضرة التي يتم تسجيل الحضور لها.
    /// يتم قراءة هذا التاريخ من عمود "Date" أو "Session Date" في ملف Excel.
    /// يجب أن يكون بتنسيق تاريخ صحيح.
    /// </summary>
    [Required(ErrorMessage = "Session date from Excel is required.")]
    public DateTime SessionDate { get; set; }

    /// <summary>
    /// يحدد ما إذا كان الطالب حاضرًا (true) أم غائبًا (false) في هذه الجلسة.
    /// يتم قراءة هذه القيمة من عمود "Present" أو "Attended" في ملف Excel.
    /// عادة ما يتم تمثيلها بـ 1 (حاضر) أو 0 (غائب)، أو "Yes"/"No"، "Present"/"Absent".
    /// عملية التحليل في ExcelProcessingService ستحول هذه القيمة إلى boolean.
    /// </summary>
    [Required(ErrorMessage = "Presence status from Excel is required.")]
    public bool IsPresent { get; set; }

    /// <summary>
    /// ملاحظات إضافية قد يسجلها الدكتور بخصوص حضور هذا الطالب في هذه الجلسة.
    /// هذا الحقل اختياري ويمكن أن يكون فارغًا.
    /// يتم قراءته من عمود "Notes" أو "Remarks" في ملف Excel (إذا وجد).
    /// </summary>
    public string? Notes { get; set; }

    // يمكن إضافة حقول أخرى إذا كان ملف Excel يحتوي على معلومات إضافية ذات صلة
    // وتريد معالجتها، مثل:
    // public string? SessionTime { get; set; } // إذا كان هناك وقت محدد للجلسة
    // public string? RoomNumber { get; set; } // إذا كان رقم القاعة مذكوراً
}