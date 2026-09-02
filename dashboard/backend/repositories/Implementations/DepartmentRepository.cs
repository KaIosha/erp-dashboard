using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class DepartmentRepository : GenericRepository<Departments>, IDepartmentsRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Departments>> GetAllOrderedByNameAsync()
        {
            return await _context.Departments.AsNoTracking().OrderBy(d => d.Name).ToListAsync();
        }
    }
}