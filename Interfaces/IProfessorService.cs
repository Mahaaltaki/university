using System;
using kalamon_University.Models.Entities;
namespace kalamon_University.Interfaces
{
    public interface IProfessorService
    {
        Task AddAsync(Professor professor);
        Task SaveChangesAsync();
    }
}