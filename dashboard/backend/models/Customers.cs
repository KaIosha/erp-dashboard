namespace backend.models
{
    public class Customers
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int? TaxId { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Orders> Orders { get; set; } = new List<Orders>();
        public ICollection<Invoices> Invoices { get; set; } = new List<Invoices>();
    }
}
