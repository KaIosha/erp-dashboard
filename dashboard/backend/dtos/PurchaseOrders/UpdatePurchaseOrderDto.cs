namespace backend.dtos
{
    public class UpdatePurchaseOrderDto
    {
        public DateTime? ExpectedDelivery { get; set; }

        public int? SupplierId { get; set; }

        public List<UpdatePurchaseLineDto>? Lines { get; set; }
    }
}
