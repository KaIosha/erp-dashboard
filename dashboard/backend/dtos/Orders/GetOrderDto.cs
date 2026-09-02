namespace backend.dtos
{
    public class GetOrderDto
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public int CustomerId { get; set; }

        public List<GetOrderLineDto> Lines { get; set; } = new();
    }
}