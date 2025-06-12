// In: kalamon_University/Helpers/MappingProfile.cs (أو أي مجلد آخر تختاره)

using AutoMapper;
using kalamon_University.DTOs.Attendance;
using kalamon_University.DTOs.Auth;
using kalamon_University.DTOs.Course;
using kalamon_University.DTOs.Admin; // افترض وجود UserDetailDto هنا
using kalamon_University.Models.Entities;

namespace kalamon_University.Helpers // <-- تأكد من أن الـ namespace صحيح
{
    // يجب أن يرث هذا الكلاس من Profile الخاص بـ AutoMapper
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- User & Auth Mappings ---
            // تحويل من بيانات التسجيل إلى نموذج المستخدم
            CreateMap<RegisterDto, User>();
            // تحويل من نموذج المستخدم إلى DTO لعرض التفاصيل
            CreateMap<User, UserDetailDto>();


            // --- Course Mappings ---
            CreateMap<Course, CourseDetailDto>().ReverseMap();
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();


            // --- Attendance Mappings ---
            // التحويل بين كيان الحضور والـ DTO الخاص به في كلا الاتجاهين
            CreateMap<Attendance, AttendanceRecordDto>().ReverseMap();
            // افترضنا أن RecordAttendanceDto هو نفس شكل AttendanceRecordDto
            CreateMap<RecordAttendanceDto, Attendance>();


            // --- Warning Mappings ---
            // يمكنك إضافة تحويلات الإنذارات هنا
            // CreateMap<Warning, WarningDto>().ReverseMap();


            // --- Notification Mappings ---
            // يمكنك إضافة تحويلات الإشعارات هنا
            // CreateMap<Notification, NotificationDto>().ReverseMap();


            // أضف أي تحويلات أخرى تحتاجها هنا
        }
    }
}