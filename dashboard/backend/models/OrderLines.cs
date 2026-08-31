namespace backend.models
{
    public class OrderLines
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }

        public int OrderId { get; set; }
        public Orders Order { get; set; } = null!;
        public int ProductId { get; set; }
        public Products Product { get; set; } = null!;
    }
}
