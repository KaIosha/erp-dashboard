using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;

namespace backend.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private IUnitOfWork _unitOfWork;
        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetCustomerDataDto> CreateCustomerAsync(CreateCustomerDto dto)
        {
            var IsExisting = await _unitOfWork.Customers.ExistsByEmailOrPhoneAsync(dto.Email, dto.Phone);
            if (IsExisting)
            {
                throw new InvalidOperationException("Customer with this email or phone number already exists.");
            }

            await _unitOfWork.Customers.AddAsync(new Customers
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                TaxId = dto.TaxId
            });

            await _unitOfWork.SaveChangesAsync();
            return new GetCustomerDataDto
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                TaxId = dto.TaxId
            };
        }
        public async Task<bool> DeleteCustomer(int id)
        {
            var customer = await _unitOfWork.Customers.GetActiveByIdAsync(id);
            if (customer is null)
                return false;
            customer.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<PaginationResultDto<GetCustomerDataDto>> GetAllCustomers(int page = 1, int pageSize = 20)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 20;

            var (customers, totalCount) = await _unitOfWork.Customers.GetPageAsync(page, pageSize);
            var data = customers.Select(c => new GetCustomerDataDto
            {
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                City = c.City,
                Country = c.Country,
                TaxId = c.TaxId
            }).ToList();

            return new PaginationResultDto<GetCustomerDataDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        public async Task<GetCustomerDataDto> GetCustomerDataByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetActiveByIdAsync(id);
            if (customer is not null)
            {
                return new GetCustomerDataDto
                {
                    Name = customer.Name,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    City = customer.City,
                    Country = customer.Country,
                    TaxId = customer.TaxId
                };
            }
            return null;
        }
        public async Task<GetCustomerDataDto> UpdateCustomerData(int id, UpdateCustomerDataDto dto)
        {
            var customer = await _unitOfWork.Customers.GetActiveByIdAsync(id);
            if (customer is null)
                return null;

            
            customer.Name = dto.Name ?? customer.Name;
            customer.Phone = dto.Phone ?? customer.Phone;
            customer.Address = dto.Address ?? customer.Address;
            customer.City = dto.City ?? customer.City;
            customer.Country = dto.Country ?? customer.Country;


            await _unitOfWork.SaveChangesAsync();
            return new GetCustomerDataDto
            {
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                City = customer.City,
                Country = customer.Country,
                TaxId = customer.TaxId
            };
        }
    }
}