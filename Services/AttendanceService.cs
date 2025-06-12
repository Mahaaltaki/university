// In: kalamon_University/Services/AttendanceService.cs

using AutoMapper;
using kalamon_University.DTOs.Attendance;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Notification;
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using kalamon_University.Models.Enums; // <-- تأكد من وجود هذا الـ namespace للـ Enum
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kalamon_University.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IWarningRepository _warningRepository;
        private readonly INotificationService _notificationService; // <-- خدمة إرسال الإشعارات
        private readonly IMapper _mapper;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            ICourseRepository courseRepository,
            IWarningRepository warningRepository,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<AttendanceService> logger)
        {
            _attendanceRepository = attendanceRepository;
            _courseRepository = courseRepository;
            _warningRepository = warningRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<AttendanceRecordDto>> RecordAttendanceAsync(RecordAttendanceDto recordDto)
        {
            try
            {
                // التحقق من أن سجل الحضور لهذا اليوم لم يتم إدخاله مسبقًا
                var alreadyExists = await _attendanceRepository.HasAttendanceRecordAsync(recordDto.StudentId, recordDto.CourseId, recordDto.SessionDate);
                if (alreadyExists)
                {
                    _logger.LogWarning("Attempted to record duplicate attendance for StudentId {StudentId} in CourseId {CourseId} on {Date}",
                        recordDto.StudentId, recordDto.CourseId, recordDto.SessionDate.Date);
                    return ServiceResult<AttendanceRecordDto>.Failed("Attendance for this student has already been recorded for this session date.");
                }

                // تسجيل الحضور
                var attendanceEntity = _mapper.Map<Attendance>(recordDto);
                await _attendanceRepository.AddAsync(attendanceEntity);
                await _attendanceRepository.SaveChangesAsync();
                _logger.LogInformation("Attendance recorded for StudentId {StudentId} in CourseId {CourseId}.", recordDto.StudentId, recordDto.CourseId);

                // إذا كان الطالب غائباً، نبدأ عملية التحقق من الإنذار التلقائي
                if (!recordDto.IsPresent)
                {
                    await CheckAndIssueAbsenceWarningAsync(recordDto.StudentId, recordDto.CourseId);
                }

                var resultDto = _mapper.Map<AttendanceRecordDto>(attendanceEntity);
                return ServiceResult<AttendanceRecordDto>.Succeeded(resultDto, "Attendance recorded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while recording attendance for StudentId {StudentId}", recordDto.StudentId);
                return ServiceResult<AttendanceRecordDto>.Failed("An unexpected error occurred.");
            }
        }

        public async Task<ServiceResult<IEnumerable<AttendanceRecordDto>>> GetAttendanceForStudentInCourseAsync(Guid studentId, int courseId)
        {
            // هذه الدالة ستحتاج إلى تعريفها في IAttendanceRepository و AttendanceRepository
            var attendanceRecords = await _attendanceRepository.GetAttendanceForStudentInCourseAsync(studentId, courseId);
            var dtos = _mapper.Map<IEnumerable<AttendanceRecordDto>>(attendanceRecords);
            return ServiceResult<IEnumerable<AttendanceRecordDto>>.Succeeded(dtos);
        }

        private async Task CheckAndIssueAbsenceWarningAsync(Guid studentId, int courseId)
        {
            // جلب بيانات المقرر لمعرفة حد الغياب
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null || course.MaxAbsenceLimit <= 0)
            {
                _logger.LogInformation("Absence warning check skipped for CourseId {CourseId}: Course not found or MaxAbsenceLimit is not set.", courseId);
                return; // لا يوجد مقرر أو لم يحدد حد للغياب
            }

            // جلب إجمالي عدد الغيابات الحالية للطالب
            int totalAbsences = await _attendanceRepository.GetAbsenceCountForStudentAsync(studentId, courseId);

            // المقارنة
            if (totalAbsences > course.MaxAbsenceLimit)
            {
                // التحقق من عدم إرسال إنذار غياب من قبل لنفس المقرر
                bool alreadyWarned = await _warningRepository.HasAbsenceWarningForCourseAsync(studentId, courseId);
                if (!alreadyWarned)
                {
                    _logger.LogInformation("Absence limit exceeded for StudentId {StudentId} in CourseId {CourseId}. Issuing a warning.", studentId, courseId);

                    // إنشاء وإرسال الإنذار الجديد
                    var newWarning = new Warning
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        Type = WarningType.Absence,
                        Message = $"تم تجاوز الحد الأقصى المسموح به للغياب ({course.MaxAbsenceLimit} غيابات) في مقرر '{course.Name}'.",
                        DateIssued = DateTime.UtcNow,
                        IssuedByUserId = null // إنذار تلقائي من النظام
                    };

                    await _warningRepository.AddAsync(newWarning);
                    await _warningRepository.SaveChangesAsync();

                    // 1. قم أولاً بإنشاء كائن الـ DTO بالبيانات المطلوبة
                    var notificationDetails = new CreateNotificationDto
                    {
                        TargetUserId = newWarning.StudentId,
                        Message = $"إنذار جديد: تجاوز حد الغياب في مقرر '{course.Name}'",
                        RelatedEntityType = "Warning", // نوع الكيان المرتبط
                        RelatedEntityId = newWarning.Id // معرف الكيان المرتبط
                    };

                    // 2. قم باستدعاء الدالة الصحيحة (SendNotificationAsync) ومرر لها الكائن
                    await _notificationService.SendNotificationAsync(notificationDetails);

                }
            }
        }
    }
}