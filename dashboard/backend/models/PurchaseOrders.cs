namespace backend.models
{
    public class PurchaseOrders
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDelivery { get; set; }

        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public ICollection<PurchaseLines> Lines { get; set; }= new List<PurchaseLines>();

        public int SupplierId { get; set; }
        public Suppliers Supplier { get; set; } = null!;

    }
}
