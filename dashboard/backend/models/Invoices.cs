namespace backend.models
{
    public class Invoices
    {
        public int Id { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime? PaidAt { get; set; }

        public int CustomerId { get; set; }
        public Customers Customer { get; set; } = null!;

        public int OrderId { get; set; }
        public Orders Order { get; set; } = null!;

        public ICollection<InvoiceLines> Lines { get; set; } = new List<InvoiceLines>();

    }
}
