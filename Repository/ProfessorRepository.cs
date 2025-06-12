using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using kalamon_University.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace kalamon_University.Repository
{
    public class ProfessorRepository : Repository<Professor, Guid>, IProfessorRepository
    {
        protected readonly AppDbContext _context;
        public ProfessorRepository(AppDbContext context) : base(context) { }

        public async Task<Professor?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Professors
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // لا حاجة لتعريف Delete هنا لأننا ورثناها من الـ Base Repository
    }
}
