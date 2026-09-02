namespace backend.dtos
{
    public class UpdateInvoiceDto
    {
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }

        public int? CustomerId { get; set; }

        public int? OrderId { get; set; }

        public List<UpdateInvoiceLineDto>? Lines { get; set; }
    }
}