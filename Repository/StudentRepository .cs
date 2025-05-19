using kalamon_University.Interfaces;
using kalamon_University.Models.Entities;
using Kalanon_University.Data;

namespace kalamon_University.Repository
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context) { }
    }
}
