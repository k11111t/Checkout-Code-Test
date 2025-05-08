using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentManager _paymentManager;
    public PaymentsController(IPaymentManager paymentManager)
    {
        _paymentManager = paymentManager;
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest request)
    {
        PostPaymentResponse? response = await _paymentManager.ProcessPaymentAsync(request);
        
        if(response.Status == PaymentStatus.Rejected)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        GetPaymentResponse? response = await _paymentManager.GetPaymentAsync(id);

        if(response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}