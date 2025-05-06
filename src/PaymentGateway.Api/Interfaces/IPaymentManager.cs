using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

public interface IPaymentManager
{
    Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request);
    Task<GetPaymentResponse> GetPaymentAsync(Guid request);
}