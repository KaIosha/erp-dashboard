namespace backend.dtos
{
    public class PayInvoiceDto
    {
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}