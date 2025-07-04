using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AutoMapper;
using kalamon_University.Models.Entities;
using kalamon_University.Interfaces;
using kalamon_University.Data;
using kalamon_University.Services;
using kalamon_University.Repository; // <-- ���� �� ���� using ����� ��� Repositories
using kalamon_University.Infrastructure;
using kalamon_University.Helpers;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// 1. DbContext Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// 2. Identity Configuration
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 3. Authentication (JWT) Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
        //  ���� �� �� ����� ��� Claims ��� ���� ���� (���� ��� �� ��������� ����������)
        //  ����� ������� ���� ���� ������
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

// 4. Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ProfessorOnly", policy => policy.RequireRole("Professor"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

// 5. Repositories & Services Dependency Injection (DI)
// ** �� ����� ��� ����� ������� **
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>)); // Generic Repo

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IWarningRepository, WarningRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IProfessorRepository, ProfessorRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IExcelProcessingService, ExcelProcessingService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IProfessorService, ProfessorService>();
builder.Services.AddScoped < kalamon_University.Interfaces.IAuthService,kalamon_University.Services.AuthService>();
builder.Services.AddScoped<IFileStorageService, LocalStorageService>();
// 6. AutoMapper Configuration
// ���� �� ������� ������ �� �� ���� ��� �� AutoMapper.Profile
builder.Services.AddAutoMapper(typeof(Program));

// 7. Controllers and API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 8. Swagger Configuration with JWT Support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Kalamon University API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseStaticFiles();

// ==>  ����� ������� ���� index.html �������� ��� ����� ������ �������
app.UseDefaultFiles();

// ...
app.UseAuthorization();
app.MapControllers(); // ��� ����� ����� ��� API Controllers ������ ��
// ��� ����� ���� ������ ����� ��� ��������
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // ������� ���� ����� ���� ��������
        await DbInitializer.SeedAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
app.Run();