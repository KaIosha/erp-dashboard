namespace backend.dtos
{
    public class CreatePurchaseLineDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }
    }
}
