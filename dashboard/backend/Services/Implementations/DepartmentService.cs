using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;

namespace backend.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<GetDepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = await _unitOfWork.Departments.GetAllOrderedByNameAsync();
            return departments.Select(d => new GetDepartmentDto
            {
                Id = d.Id,
                Name = d.Name
            }).ToList();
        }
    }
}