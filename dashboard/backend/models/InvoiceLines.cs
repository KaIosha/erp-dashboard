namespace backend.models
{
    public class InvoiceLines
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public int InvoiceId { get; set; }
        public Invoices Invoice { get; set; } = null!;
    }
}
