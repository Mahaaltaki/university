// في ملف Data/DbInitializer.cs
using Microsoft.AspNetCore.Identity;
using kalamon_University.Models.Entities;
using kalamon_University.Models.Enums; // للتأكد من وجود enum Role

namespace kalamon_University.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            // الحصول على الخدمات المطلوبة
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            
           
            // 1. إنشاء المستخدم المسؤول (Admin User)
            var adminEmail = "admin@university.com"; // <-- هذا هو إيميل الدخول
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            // إذا لم يكن هناك مستخدم مسؤول، قم بإنشائه
            if (adminUser == null)
            {
                var newAdminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    EmailConfirmed = true, // مهم جدًا لتسجيل الدخول مباشرة
                    Role = Role.Admin // تعيين الدور في الموديل الخاص بك
                };

                // كلمة المرور الافتراضية للمسؤول
                string adminPassword = "AdminPassword123!"; // <-- هذه هي كلمة المرور

                var createAdminResult = await userManager.CreateAsync(newAdminUser, adminPassword);

                // إذا نجح إنشاء المستخدم، قم بإضافته إلى دور "Admin"
                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }
        }
    }
}