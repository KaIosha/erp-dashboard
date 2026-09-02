namespace backend.dtos
{
    public class GetPurchaseOrderDto
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDelivery { get; set; }

        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public int SupplierId { get; set; }

        public List<GetPurchaseLineDto> Lines { get; set; } = new();
    }
}
