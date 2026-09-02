namespace backend.dtos
{
    public class CreatePurchaseOrderDto
    {
        public DateTime ExpectedDelivery { get; set; }

        public int SupplierId { get; set; }

        public List<CreatePurchaseLineDto> Lines { get; set; } = new();
    }
}
