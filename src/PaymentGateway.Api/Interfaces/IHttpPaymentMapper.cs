using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces;
public interface IHttpPaymentMapper
{
    public PostPaymentResponse? CreatePostResponse(PaymentRecord payment);
    public GetPaymentResponse? CreateGetResponse(PaymentRecord payment);
}