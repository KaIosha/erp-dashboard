namespace backend.models
{
    public class Products
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }

        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<PurchaseLines> PurchaseLines { get; set; } = new List<PurchaseLines>();

        public ICollection<OrderLines> OrderLines { get; set; } = new List<OrderLines>();

        public int CategoryId { get; set; }
        public Categories Category { get; set; } = null!;

    }
}
