namespace backend.dtos
{
    public class CreateOrderDto
    {
        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;

        public List<CreateOrderLineDto> Lines { get; set; } = new();
    }
}