using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;

namespace backend.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetSupplierDto> CreateSupplierAsync(CreateSupplierDto dto)
        {
            var IsExistingSupplier = await _unitOfWork.Suppliers.ExistsByEmailAsync(dto.Email);
            if (IsExistingSupplier)
            {
                throw new InvalidOperationException("Supplier with the same email already exists.");
            }

            var supplier = new Suppliers
            {
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                PaymentTerms = dto.PaymentTerms
            };
            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();
            return new GetSupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                Phone = supplier.Phone,
                PaymentTerms = supplier.PaymentTerms
            };
        }
        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return false;

            _unitOfWork.Suppliers.Remove(supplier);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<PaginationResultDto<GetSupplierDto>> GetAllSuppliersAsync(string? search = null, int page = 1, int pageSize = 20)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 20;

            var (suppliers, totalCount) = await _unitOfWork.Suppliers.GetPageAsync(search, page, pageSize);
            var data = suppliers.Select(s => new GetSupplierDto
            {
                Id = s.Id,
                Email = s.Email,
                Phone = s.Phone,
                CompanyName = s.CompanyName,
                ContactName = s.ContactName,
                PaymentTerms = s.PaymentTerms
            }).ToList();

            return new PaginationResultDto<GetSupplierDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        public async Task<GetSupplierDto?> GetSupplierByIdAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier is null)
                return null;

            return new GetSupplierDto
            {
                Id = supplier.Id,
                Email = supplier.Email,
                Phone = supplier.Phone,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                PaymentTerms = supplier.PaymentTerms
            };
        }
        public async Task<GetSupplierDto?> UpdateSupplierAsync(int id, UpdateSupplierDto dto)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return null;

            supplier.CompanyName = dto.CompanyName ?? supplier.CompanyName;
            supplier.ContactName = dto.ContactName ?? supplier.ContactName;
            supplier.Email = dto.Email ?? supplier.Email;
            supplier.Phone = dto.Phone ?? supplier.Phone;
            supplier.PaymentTerms = dto.PaymentTerms ?? supplier.PaymentTerms;

            await _unitOfWork.SaveChangesAsync();

            return new GetSupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                Phone = supplier.Phone,
                PaymentTerms = supplier.PaymentTerms
            };
        }
    }
}