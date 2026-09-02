using backend.dtos;
using backend.Services.Interfaces;
using backend.Validation.Invoice;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _service;
        private readonly IValidator<CreateInvoiceDto> _createValidator;
        private readonly IValidator<UpdateInvoiceDto> _updateValidator;
        private readonly IValidator<PayInvoiceDto> _payValidator;

        public InvoicesController(
            IInvoiceService service,
            IValidator<CreateInvoiceDto> createValidator,
            IValidator<UpdateInvoiceDto> updateValidator,
            IValidator<PayInvoiceDto> payValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _payValidator = payValidator;
        }

        [Authorize(Policy = "invoices:view")]
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices([FromQuery] string? search = null,
            [FromQuery] string? status = null, [FromQuery] int? customerId = null,
            [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllInvoicesAsync(search, status, customerId, from, to, page, pageSize);
            return Ok(result);
        }

        [Authorize(Policy = "invoices:view")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await _service.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound("Invoice not found");

            return Ok(invoice);
        }

        [Authorize(Policy = "invoices:manage")]
        [HttpGet("{id:int}/pdf")]
        public async Task<IActionResult> GetInvoicePdf(int id)
        {
            var pdfData = await _service.GetInvoicePdfAsync(id);
            if (pdfData == null)
                return NotFound("Invoice not found");

            return File(pdfData, "application/pdf", $"invoice_{id}.pdf");
        }

        [Authorize(Policy = "invoices:manage")]
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var invoice = await _service.CreateInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }

        [Authorize(Policy = "invoices:manage")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] UpdateInvoiceDto dto)
        {
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var invoice = await _service.UpdateInvoiceAsync(id, dto);
            if (invoice == null)
                return NotFound("Invoice not found");

            return Ok(invoice);
        }

        [Authorize(Policy = "invoices:pay")]
        [HttpPatch("{id:int}/pay")]
        public async Task<IActionResult> PayInvoice(int id, [FromBody] PayInvoiceDto dto)
        {
            var validationResult = _payValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var invoice = await _service.PayInvoiceAsync(id, dto);
            if (invoice == null)
                return NotFound("Invoice not found");
            return Ok(invoice);
        }

        [Authorize(Policy = "invoices:manage")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _service.DeleteInvoiceAsync(id);
            if (!result)
                return NotFound("Invoice not found");

            return Ok("Invoice deleted successfully");
        }
    }
}