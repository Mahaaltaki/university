using kalamon_University.Data;
using Microsoft.EntityFrameworkCore;
using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using kalamon_University.Repository;

public class StudentRepository : Repository<Student, Guid>, IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public void Delete(Student student)
    {
        _context.Students.Remove(student);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Student?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Students
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
