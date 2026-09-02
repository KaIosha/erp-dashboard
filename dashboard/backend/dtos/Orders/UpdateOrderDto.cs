namespace backend.dtos
{
    public class UpdateOrderDto
    {
        public int? CustomerId { get; set; }

        public string? PaymentMethod { get; set; }
        public string? ShippingAddress { get; set; }

        public List<UpdateOrderLineDto>? Lines { get; set; }
    }
}