using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace kalamon_University.Interfaces

public interface IAdminService
{
    Task<IEnumerable<UserDetailDto>> GetAllStudentsAsync();
    Task<UserDetailDto?> GetStudentByIdAsync(Guid id);
    Task<UserDetailDto> CreateStudentAsync(CreateUserDto studentDto);
    Task<bool> UpdateStudentAsync(Guid id, UpdateUserDto studentDto);
    Task<bool> DeleteStudentAsync(Guid id);

    Task<IEnumerable<UserDetailDto>> GetAllDoctorsAsync();
    Task<UserDetailDto?> GetDoctorByIdAsync(Guid id);
    Task<UserDetailDto> CreateDoctorAsync(CreateUserDto doctorDto);
    Task<bool> UpdateDoctorAsync(Guid id, UpdateUserDto doctorDto);
    Task<bool> DeleteDoctorAsync(Guid id);
    // ... other admin functionalities like course management
}