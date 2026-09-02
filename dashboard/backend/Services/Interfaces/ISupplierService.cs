using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<PaginationResultDto<GetSupplierDto>> GetAllSuppliersAsync(string? search = null, int page = 1, int pageSize = 20);
        Task<GetSupplierDto?> GetSupplierByIdAsync(int id);

        Task<GetSupplierDto> CreateSupplierAsync(CreateSupplierDto dto);

        Task<GetSupplierDto?> UpdateSupplierAsync(int id,UpdateSupplierDto dto);

        Task<bool> DeleteSupplierAsync(int id);
    }
}
