using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<PaginationResultDto<GetInvoiceDto>> GetAllInvoicesAsync(
            string? search = null,
            string? status = null,
            int? customerId = null,
            DateTime? from = null,
            DateTime? to = null,
            int page = 1,
            int pageSize = 20);
        Task<GetInvoiceDto?> GetInvoiceByIdAsync(int id);

        Task<byte[]?> GetInvoicePdfAsync(int id);

        Task<GetInvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto);

        Task<GetInvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto dto);

        Task<GetInvoiceDto?> PayInvoiceAsync(int id, PayInvoiceDto dto);

        Task<bool> DeleteInvoiceAsync(int id);
    }
}