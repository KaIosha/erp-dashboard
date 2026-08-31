namespace backend.models
{
    public class PurchaseLines
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }


        public int ProductId { get; set; }
        public Products Product { get; set; } = null!;
        public int PurchaseOrderId { get; set; }
        public PurchaseOrders PurchaseOrder { get; set; } = null!;
    }
}
