namespace backend.dtos
{
    public class GetInvoiceDto
    {
        public int Id { get; set; }

        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? PaymentMethod { get; set; }

        public int CustomerId { get; set; }
        public int? OrderId { get; set; }

        public List<GetInvoiceLineDto> Lines { get; set; } = new();
    }
}