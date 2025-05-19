using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using Kalanon_University.Data;

namespace kalamon_University.Repository
{
    public class ProfessorRepository : GenericRepository<Professor>, IProfessorRepository
    {
        public ProfessorRepository(AppDbContext context) : base(context) { }
    }
}
