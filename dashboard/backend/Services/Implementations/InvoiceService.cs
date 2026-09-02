using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;

namespace backend.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginationResultDto<GetInvoiceDto>> GetAllInvoicesAsync(string? search = null,
            string? status = null, int? customerId = null, DateTime? from = null,
            DateTime? to = null, int page = 1, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (invoices, totalCount) = await _unitOfWork.Invoices.GetPageAsync(search, status, customerId, from, to, page, pageSize);
            var data = invoices.Select(i => MapInvoice(i)).ToList();

            return new PaginationResultDto<GetInvoiceDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<byte[]?> GetInvoicePdfAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdWithCustomerAndLinesAsync(id);

            if (invoice == null)
                return null;

            using (PdfDocument document = new PdfDocument())
            {
                document.Info.Title = $"Invoice {invoice.Id}";

                PdfPage page = document.AddPage();

                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    XFont titleFont = new XFont("Arial", 20);
                    XFont headingFont = new XFont("Arial", 14);
                    XFont bodyFont = new XFont("Arial", 12);

                    double width = page.Width - 80;
                    double y = 100;

                    gfx.DrawString("INVOICE", titleFont, XBrushes.Black, new XRect(40, 40, width, 40), XStringFormats.TopCenter);
                    gfx.DrawString($"Invoice No: {invoice.Id}", bodyFont, XBrushes.Black, 40, y);
                    gfx.DrawString($"Date: {invoice.InvoiceDate:yyyy-MM-dd}", bodyFont, XBrushes.Black, 40, y + 20);
                    gfx.DrawString($"Customer: {invoice.Customer.Name}", bodyFont, XBrushes.Black, 40, y + 40);

                    gfx.DrawLine(XPens.DarkGray, 40, 190, page.Width - 40, 190);

                    y = 210;
                    gfx.DrawString("Product", headingFont, XBrushes.Black, 40, y);
                    gfx.DrawString("Quantity", headingFont, XBrushes.Black, 250, y);
                    gfx.DrawString("Unit Price", headingFont, XBrushes.Black, 350, y);
                    gfx.DrawString("Total", headingFont, XBrushes.Black, 470, y);

                    y += 30;
                    foreach (var line in invoice.Lines)
                    {
                        var lineTotal = line.Quantity * line.UnitPrice * (1 + line.TaxRate / 100m);

                        gfx.DrawString(line.Description, bodyFont, XBrushes.Black, 40, y);
                        gfx.DrawString(line.Quantity.ToString(), bodyFont, XBrushes.Black, 250, y);
                        gfx.DrawString($"{line.UnitPrice:N2}", bodyFont, XBrushes.Black, 350, y);
                        gfx.DrawString($"{lineTotal:N2}", bodyFont, XBrushes.Black, 470, y);

                        y += 25;
                    }

                    y += 15;
                    gfx.DrawLine(XPens.DarkGray, 40, y, page.Width - 40, y);

                    y += 35;
                    gfx.DrawString("Total Amount Due:", headingFont, XBrushes.Black, 40, y);
                    gfx.DrawString($"${invoice.TotalAmount:N2}", titleFont, XBrushes.ForestGreen, 200, y);

                    gfx.DrawString("Have a nice day!", bodyFont, XBrushes.Black, new XRect(40, page.Height - 70, width, 30), XStringFormats.Center);
                }

                await using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    return stream.ToArray();
                }
            }
        }

        public async Task<GetInvoiceDto?> PayInvoiceAsync(int id, PayInvoiceDto dto)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdWithLinesAsync(id);

            if (invoice is null)
                return null;

            invoice.Status = "Paid";
            invoice.PaidAt = dto.PaymentDate;
            invoice.PaymentMethod = dto.PaymentMethod;

            await _unitOfWork.SaveChangesAsync();

            return MapInvoice(invoice);
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdWithLinesAsync(id);

            if (invoice == null)
                return false;

            if (invoice.Status != "Draft")
                return false;

            _unitOfWork.Repository<InvoiceLines>().RemoveRange(invoice.Lines);
            _unitOfWork.Invoices.Remove(invoice);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<GetInvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            var invoice = new Invoices
            {
                InvoiceDate = dto.InvoiceDate,
                DueDate = dto.DueDate,
                Status = "Draft",
                TotalAmount = dto.Lines.Sum(l => l.Quantity * l.UnitPrice * (1 + l.TaxRate / 100m)),
                CustomerId = dto.CustomerId,
                OrderId = dto.OrderId,
                Lines = dto.Lines.Select(l => new InvoiceLines
                {
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    TaxRate = l.TaxRate
                }).ToList()
            };

            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            return MapInvoice(invoice);
        }

        public async Task<GetInvoiceDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdWithLinesAsync(id);
            if (invoice is null)
                return null;

            return MapInvoice(invoice);
        }

        public async Task<GetInvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto dto)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdWithLinesAsync(id);

            if (invoice is null)
                return null;

            if (dto.InvoiceDate.HasValue)
                invoice.InvoiceDate = dto.InvoiceDate.Value;

            if (dto.DueDate.HasValue)
                invoice.DueDate = dto.DueDate.Value;

            if (dto.CustomerId.HasValue)
                invoice.CustomerId = dto.CustomerId.Value;

            if (dto.OrderId.HasValue)
                invoice.OrderId = dto.OrderId.Value;

            if (dto.Lines is not null)
            {
                var dtoLineIds = dto.Lines.Where(l => l.Id > 0).Select(l => l.Id).ToHashSet();

                foreach (var line in invoice.Lines.Where(l => !dtoLineIds.Contains(l.Id)).ToList())
                {
                    invoice.Lines.Remove(line);
                }

                foreach (var lineDto in dto.Lines)
                {
                    var line = invoice.Lines.FirstOrDefault(l => l.Id == lineDto.Id);

                    if (line is not null)
                    {
                        line.Description = lineDto.Description;
                        line.Quantity = lineDto.Quantity;
                        line.UnitPrice = lineDto.UnitPrice;
                        line.TaxRate = lineDto.TaxRate;
                    }
                    else
                    {
                        invoice.Lines.Add(new InvoiceLines
                        {
                            Description = lineDto.Description,
                            Quantity = lineDto.Quantity,
                            UnitPrice = lineDto.UnitPrice,
                            TaxRate = lineDto.TaxRate
                        });
                    }
                }
            }

            invoice.TotalAmount = invoice.Lines
                .Sum(l => l.Quantity * l.UnitPrice * (1 + l.TaxRate / 100m));

            await _unitOfWork.SaveChangesAsync();

            return MapInvoice(invoice);
        }

        private static GetInvoiceDto MapInvoice(Invoices invoice) => new GetInvoiceDto
        {
            Id = invoice.Id,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            TotalAmount = invoice.TotalAmount,
            PaidAt = invoice.PaidAt,
            PaymentMethod = invoice.PaymentMethod,
            CustomerId = invoice.CustomerId,
            OrderId = invoice.OrderId,
            Lines = invoice.Lines.Select(l => new GetInvoiceLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TaxRate = l.TaxRate
            }).ToList()
        };
    }
}