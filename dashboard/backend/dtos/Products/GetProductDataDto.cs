using backend.models;

namespace backend.dtos
{
    public class GetProductDataDto
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public int CategoryId { get; set; }
        public Categories Category { get; set; } = null!;
    }
}
