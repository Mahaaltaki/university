// kalamon_University.Core/Interfaces/IExcelProcessingService.cs
using kalamon_University.DTOs.Attendance; // For AttendanceRecordDto or similar import DTO
using kalamon_University.DTOs.Course; // For StudentBasicInfoDto or similar export DTO
using kalamon_University.DTOs.Common;

// --- DTOs for Excel Processing ---
// (Define these in kalamon_University/DTOs/Excel/ or relevant DTO folders)
// namespace kalamon_University.DTOs.Excel;
// public record StudentBasicInfoDto(string StudentIdNumber, string Name); // For generating attendance sheet
// public record ExcelAttendanceRecordDto(string StudentId, DateTime SessionDate, bool IsPresent, string? Notes); // For parsing attendance sheet

namespace kalamon_University.Core.Interfaces.Services;

using System.Collections.Generic;
using System.IO;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Excel; // Assuming DTOs are in this namespace

public interface IExcelProcessingService
{
    /// <summary>
    /// Generates an Excel file (as byte array) for an attendance sheet for a given course.
    /// The sheet will list students and have columns for the Professor to fill in attendance.
    /// </summary>
    /// <param name="students">List of students to include in the sheet.</param>
    /// <param name="courseId">Identifier for the course (for naming or context).</param>
    /// <returns>A ServiceResult containing the byte array of the Excel file or errors.</returns>
    ServiceResult<byte[]> GenerateAttendanceSheetTemplate(IEnumerable<StudentBasicInfoDto> students, int courseId);

    /// <summary>
    /// Parses an uploaded Excel attendance sheet.
    /// </summary>
    /// <param name="excelStream">The stream of the uploaded Excel file.</param>
    /// <param name="courseId">The course ID to associate the attendance records with.</param>
    /// <returns>A ServiceResult containing a list of parsed attendance records or errors.</returns>
    ServiceResult<IEnumerable<ExcelAttendanceRecordDto>> ParseAttendanceSheet(Stream excelStream, int courseId);

    // Potentially other Excel generation/parsing methods:
    // ServiceResult<byte[]> GenerateStudentGradesReport(int courseId, IEnumerable<StudentGradeDto> grades);
    // ServiceResult<IEnumerable<NewStudentDto>> ParseStudentImportSheet(Stream excelStream);
}