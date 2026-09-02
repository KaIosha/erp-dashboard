using backend.models;

namespace backend.dtos
{
    public class UpdateProductDataDto
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public int? StockQuantity { get; set; }
        public int? ReorderLevel { get; set; }
    }
}