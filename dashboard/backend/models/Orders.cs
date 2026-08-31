namespace backend.models
{
    public class Orders
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;

        public string ShippingAddress { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public ICollection<OrderLines> Lines { get; set; } = new List<OrderLines>();

        public int CustomerId { get; set; }
        public Customers Customer { get; set; } = null!;
        public Invoices? Invoice { get; set; }


    }
}
