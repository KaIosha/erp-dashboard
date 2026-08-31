namespace backend.models
{
    public class Suppliers 
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string PaymentTerms { get; set; } = string.Empty;
        public ICollection<PurchaseOrders> PurchaseOrders { get; set; } = new List<PurchaseOrders>();

    }
}
