using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;

namespace backend.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginationResultDto<GetEmployeeDto>> GetAllEmployeesAsync(
            string? search = null,
            string? department = null,
            int page = 1,
            int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (employees, totalCount) = await _unitOfWork.Employees.GetPageAsync(search, department, page, pageSize);
            var data = employees.Select(e => MapEmployee(e)).ToList();

            return new PaginationResultDto<GetEmployeeDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<GetEmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetActiveByIdAsync(id);
            if (employee is null)
                return null;

            return MapEmployee(employee);
        }

        public async Task<GetEmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            var departmentExists = await _unitOfWork.Departments.ExistsAsync(dto.DepartmentId);

            if (!departmentExists)
                throw new InvalidOperationException("Department not found.");

            var employee = new Employees
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DepartmentId = dto.DepartmentId,
                Position = dto.Position,
                HireDate = dto.HireDate,
                Salary = dto.Salary
            };

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return (await GetEmployeeByIdAsync(employee.Id))!;
        }

        public async Task<GetEmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
        {
            var employee = await _unitOfWork.Employees.GetActiveByIdAsync(id);

            if (employee is null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                employee.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                employee.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                employee.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                employee.Phone = dto.Phone;

            if (!string.IsNullOrWhiteSpace(dto.Position))
                employee.Position = dto.Position;

            if (dto.HireDate.HasValue)
                employee.HireDate = dto.HireDate.Value;

            if (dto.Salary.HasValue)
                employee.Salary = dto.Salary.Value;

            if (dto.DepartmentId.HasValue)
            {
                var departmentExists = await _unitOfWork.Departments.ExistsAsync(dto.DepartmentId.Value);

                if (!departmentExists)
                    throw new InvalidOperationException("Department not found.");

                employee.DepartmentId = dto.DepartmentId.Value;
            }

            await _unitOfWork.SaveChangesAsync();

            return await GetEmployeeByIdAsync(id);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetActiveByIdAsync(id);

            if (employee is null)
                return false;

            employee.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static GetEmployeeDto MapEmployee(Employees e) => new GetEmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Phone = e.Phone,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department.Name,
            Position = e.Position,
            HireDate = e.HireDate,
            Salary = e.Salary
        };
    }
}