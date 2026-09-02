namespace backend.dtos
{
    public class CreateInvoiceDto
    {
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }

        public int CustomerId { get; set; }

        public int? OrderId { get; set; }

        public List<CreateInvoiceLineDto> Lines { get; set; } = new();
    }
}