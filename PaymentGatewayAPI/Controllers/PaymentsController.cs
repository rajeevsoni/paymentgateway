using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Data;
using PaymentGateway.Data.Models;
using PaymentGatewayAPI.Services;

namespace PaymentGatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentGatewayService paymentGatewayService, ILogger<PaymentsController> logger)
        {
            _paymentGatewayService = paymentGatewayService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentRequest paymentRequest)
        {
            _logger.LogInformation($"Entering {nameof(PaymentsController)} - {nameof(Post)}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PaymentResponse paymentResponse = await _paymentGatewayService.SubmitPaymentRequest(paymentRequest);
            return Accepted(paymentResponse);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> Get(string paymentId)
        {
            _logger.LogInformation($"Entering {nameof(PaymentsController)} - {nameof(Get)} with {paymentId}");

            Guid.TryParse(paymentId, out Guid paymentIdentifier);
            PaymentDetails paymentDetails = await _paymentGatewayService.GetPaymentDetails(paymentIdentifier);
            if (paymentDetails == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(paymentDetails);
            }
        }
    }
}