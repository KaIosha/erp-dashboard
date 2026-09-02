namespace backend.dtos
{
    public class UpdatePurchaseLineDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }
    }
}
