
using System;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Admin;

// DTO أساسي لإنشاء مستخدم (مشترك بين الطالب والدكتور)
public abstract class CreateUserBaseDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; set; }="null";

    public string? UserName { get; set; } // اختياري، يمكن أن يكون نفس الإيميل
    public bool EmailConfirmed { get; set; } = true; // الأدمن قد يؤكد الإيميل مباشرة
}

// DTO لإنشاء طالب بواسطة الأدمن
public class CreateStudentByAdminDto : CreateUserBaseDto
{
    [Required(ErrorMessage = "Student ID number is required.")]
    public string UserId { get; set; }
    
}

// DTO لإنشاء دكتور بواسطة الأدمن
public class CreateProfessorByAdminDto : CreateUserBaseDto
{
    [Required(ErrorMessage = "Staff ID number is required.")]
    public string UserId { get; set; }
    public string? Specialization { get; set; }
    // يمكنك إضافة أي حقول أخرى خاصة بالدكتور يريد الأدمن إدخالها عند الإنشاء
    // public string? OfficeNumber { get; set; }
}