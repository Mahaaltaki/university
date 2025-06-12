using Microsoft.Extensions.Logging; // لاستخدام ILogger
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization; // لـ CultureInfo و DateTimeStyles
using System.IO;
using System.Linq;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Excel;
using kalamon_University.Interfaces;

namespace kalamon_University.Infrastructure;

public class ExcelProcessingService : IExcelProcessingService
{
    private readonly ILogger<ExcelProcessingService> _logger;

    // مصفوفة بتنسيقات التواريخ المتوقعة
    private static readonly string[] ExpectedDateFormats = new[]
    {
        "yyyy-MM-dd", "dd-MM-yyyy", "MM/dd/yyyy", "yyyy/MM/dd",
        "dd/MM/yyyy", "M/d/yyyy", "d/M/yyyy",
        "yyyy.MM.dd", "dd.MM.yyyy"
        // يمكنك إضافة المزيد من التنسيقات الشائعة هنا
    };

    public ExcelProcessingService(ILogger<ExcelProcessingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ServiceResult<byte[]> GenerateAttendanceSheetTemplate(IEnumerable<kalamon_University.DTOs.Student.StudentProfileDto> students, int courseId)
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add($"Attendance_Course_{courseId}");

        // تعريف العناوين المتوقعة
        var headers = new Dictionary<int, string>
        {
            {1, "Student ID "},
            {2, "Student Name"},
            {3, "Session Date (YYYY-MM-DD)"}, // إعطاء تلميح للتنسيق المفضل
            {4, "Is Present (1=Yes, 0=No)"},
            {5, "Notes"}
        };

        foreach (var header in headers)
        {
            worksheet.Cells[1, header.Key].Value = header.Value;
        }
        worksheet.Cells[1, 1, 1, headers.Count].Style.Font.Bold = true; // جعل العناوين غامقة

        int currentRow = 2;
        foreach (var student in students)
        {
            worksheet.Cells[currentRow, 1].Value = student.StudentIdNumber;
            worksheet.Cells[currentRow, 2].Value = student.FullName;
            currentRow++;
        }

        // ضبط عرض الأعمدة تلقائيًا
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();


        try
        {
            _logger.LogInformation("Successfully generated attendance sheet template for course ID {CourseId}.", courseId);
            return ServiceResult<byte[]>.Succeeded(package.GetAsByteArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating Excel template for course ID {CourseId}.", courseId);
            return ServiceResult<byte[]>.Failed($"Error generating Excel template: {ex.Message}");
        }
    }

    public ServiceResult<IEnumerable<ExcelStudentAttendanceDto>> ParseAttendanceSheet(Stream excelStream, int courseId)
    {
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        var records = new List<ExcelStudentAttendanceDto>();
        var validationErrors = new List<string>(); // لتجميع أخطاء التحقق من الصحة لكل صف

        try
        {
            using var package = new ExcelPackage(excelStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                _logger.LogWarning("Excel sheet not found in the uploaded file for course ID {CourseId}.", courseId);
                return ServiceResult<IEnumerable<ExcelStudentAttendanceDto>>.Failed("Excel sheet not found in the uploaded file.");
            }

            // التحقق من صحة العناوين (اختياري ولكنه جيد)
            var expectedHeaders = new Dictionary<int, string>
            {
                {1, "Student ID Number"}, {2, "Student Name"}, {3, "Session Date"}, {4, "Is Present"}, {5, "Notes"}
            };
            bool headersValid = true;
            for (int col = 1; col <= Math.Min(expectedHeaders.Count, worksheet.Dimension.End.Column); col++)
            {
                var headerText = worksheet.Cells[1, col].Text?.Trim();
                // تحقق مرن قليلاً للعناوين
                if (expectedHeaders.ContainsKey(col) &&
                    !string.IsNullOrWhiteSpace(headerText) &&
                    !headerText.Contains(expectedHeaders[col], StringComparison.OrdinalIgnoreCase))
                {
                    // إذا كان العمود 5 (Notes) فارغًا في الهيدر ولكنه موجود كعمود، لا نعتبره خطأ فادح
                    if (col == 5 && string.IsNullOrWhiteSpace(headerText) && worksheet.Dimension.End.Column >= 5)
                    {
                        // لا بأس
                    }
                    else
                    {
                        _logger.LogWarning("Header mismatch for course ID {CourseId}. Column {Column}: Expected something like '{ExpectedHeader}', Got '{ActualHeader}'.",
                                       courseId, col, expectedHeaders[col], headerText);
                        validationErrors.Add($"Header in column {col} ('{headerText}') does not match expected ('{expectedHeaders[col]}'). Please use the template.");
                        // headersValid = false; // يمكنك اختيار إيقاف المعالجة هنا إذا كانت العناوين غير صحيحة تمامًا
                    }
                }
            }
            // if (!headersValid) return ServiceResult<IEnumerable<ExcelStudentAttendanceDto>>.Failed(validationErrors);


            int startRow = 2; // البيانات تبدأ من الصف الثاني

            for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
            {
                // التحقق مما إذا كان الصف فارغًا بالكامل لتجنب معالجة صفوف فارغة في نهاية الملف
                bool isRowEmpty = true;
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                    {
                        isRowEmpty = false;
                        break;
                    }
                }
                if (isRowEmpty) continue;


                var studentIdFromExcel = worksheet.Cells[row, 1].GetValue<string>()?.Trim();
                var studentNameFromExcel = worksheet.Cells[row, 2].GetValue<string>()?.Trim();
                var sessionDateStr = worksheet.Cells[row, 3].Text?.Trim(); // استخدام .Text أفضل للتواريخ
                var isPresentStr = worksheet.Cells[row, 4].GetValue<string>()?.Trim();
                var notesFromExcel = worksheet.Cells[row, 5].GetValue<string>()?.Trim();

                // التحقق من الحقول المطلوبة
                if (string.IsNullOrWhiteSpace(studentIdFromExcel))
                {
                    validationErrors.Add($"Row {row}: Student ID Number is missing.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(sessionDateStr))
                {
                    validationErrors.Add($"Row {row} (Student ID: {studentIdFromExcel}): Session Date is missing.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(isPresentStr))
                {
                    validationErrors.Add($"Row {row} (Student ID: {studentIdFromExcel}): Presence status is missing.");
                    continue;
                }

                // معالجة التاريخ المحسنة
                DateTime sessionDate;
                bool dateParsed = false;
                // 1. محاولة التحويل باستخدام التنسيقات المحددة
                if (DateTime.TryParseExact(sessionDateStr, ExpectedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out sessionDate))
                {
                    dateParsed = true;
                }
                // 2. محاولة التحويل العامة
                else if (DateTime.TryParse(sessionDateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out sessionDate))
                {
                    dateParsed = true;
                }
                // 3. محاولة التحويل من رقم Excel التسلسلي (OADate)
                else if (double.TryParse(sessionDateStr, out double oaDate))
                {
                    try
                    {
                        sessionDate = DateTime.FromOADate(oaDate);
                        dateParsed = true;
                    }
                    catch
                    {
                        // فشل تحويل OADate
                    }
                }

                if (!dateParsed)
                {
                    validationErrors.Add($"Row {row} (Student ID: {studentIdFromExcel}): Invalid date format '{sessionDateStr}'. Please use a recognizable date format (e.g., YYYY-MM-DD).");
                    continue;
                }


                // معالجة حالة الحضور المحسنة (غير حساسة لحالة الأحرف)
                bool isPresent;
                string lowerIsPresentStr = isPresentStr.ToLowerInvariant();
                if (lowerIsPresentStr == "1" || lowerIsPresentStr == "yes" || lowerIsPresentStr == "true" || lowerIsPresentStr == "present" || lowerIsPresentStr == "حاضر")
                {
                    isPresent = true;
                }
                else if (lowerIsPresentStr == "0" || lowerIsPresentStr == "no" || lowerIsPresentStr == "false" || lowerIsPresentStr == "absent" || lowerIsPresentStr == "غائب")
                {
                    isPresent = false;
                }
                else
                {
                    validationErrors.Add($"Row {row} (Student ID: {studentIdFromExcel}): Invalid presence value '{isPresentStr}'. Expected 1/0, Yes/No, True/False, Present/Absent, حاضر/غائب.");
                    continue;
                }

                records.Add(new ExcelStudentAttendanceDto
                {
                    StudentId = studentIdFromExcel,
                    StudentName = string.IsNullOrWhiteSpace(studentNameFromExcel) ? null : studentNameFromExcel,
                    SessionDate = sessionDate,
                    IsPresent = isPresent,
                    Notes = string.IsNullOrWhiteSpace(notesFromExcel) ? null : notesFromExcel
                });
            } // نهاية حلقة for

            if (validationErrors.Any())
            {
                _logger.LogWarning("Validation errors encountered while parsing attendance sheet for course ID {CourseId}. Errors: {ValidationErrors}", courseId, string.Join("; ", validationErrors));
                // إذا كنت تريد إرجاع السجلات الناجحة مع الأخطاء
                var result = ServiceResult<IEnumerable<ExcelStudentAttendanceDto>>.Succeeded(records, "Data parsed with some validation errors.");
                validationErrors.ForEach(e => result.AddError(e)); // نفترض وجود دالة AddError في ServiceResult
                return result;
                // أو إذا كنت تريد الفشل إذا كان هناك أي خطأ
                // return ServiceResult<IEnumerable<ExcelStudentAttendanceDto>>.Failed(validationErrors);
            }

            if (!records.Any() && worksheet.Dimension.End.Row >= startRow) // إذا لم يتم تحليل أي سجلات ولكن كان هناك صفوف بيانات
            {
                _logger.LogWarning("No valid records found in the attendance sheet for course ID {CourseId}, though rows were present.", courseId);
                return ServiceResult<IEnumerable<ExcelStudentAttendanceDto>>.Failed("No valid student attendance records found in the uploaded file. Please check the data and format.");
            }


            _logger.LogInformation("Successfully parsed {RecordCount} attendance records from Excel for course ID {CourseId}.", records.Count, courseId);
            return ServiceResult<IEnumerable<ExcelStudentAttendanceDto>>.Succeeded(records);
        }
        catch (InvalidDataException ex) // EPPlus قد يرمي هذا إذا كان الملف تالفًا
        {
            _logger.LogError(ex, "Invalid Excel file format or corrupted file for course ID {CourseId}.", courseId);
            return ServiceResult<IEnumerable<ExcelStudentAttendanceDto>>.Failed($"Invalid Excel file format or corrupted file: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while parsing the Excel file for course ID {CourseId}.", courseId);
            return ServiceResult<IEnumerable<ExcelStudentAttendanceDto>>.Failed($"An unexpected error occurred: {ex.Message}");
        }
    }
}

