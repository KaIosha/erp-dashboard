using backend.Data;
using backend.models;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.repositories.Implementations
{
    public class EmployeeRepository : GenericRepository<Employees>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(List<Employees> Data, int TotalCount)> GetPageAsync(string? search, string? department, int page, int pageSize)
        {
            var query = _context.Employees.AsNoTracking().Include(e => e.Department).Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(search) ||
                    e.LastName.ToLower().Contains(search) ||
                    e.Email.ToLower().Contains(search) ||
                    e.Position.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                department = department.ToLower();
                query = query.Where(e => e.Department.Name.ToLower().Contains(department));
            }

            var totalCount = await query.CountAsync();
            var data = await query
                .OrderByDescending(e => e.HireDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }

        public async Task<Employees?> GetActiveByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}