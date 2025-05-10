using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.Data;
public class HttpPaymentMapper : IHttpPaymentMapper
{
    public PostPaymentResponse? CreatePostResponse(PaymentRecord payment)
    {
        if(payment == null)
            return null;

        return new PostPaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency ?? string.Empty,
            Amount = payment.Amount
        };
    }

    public GetPaymentResponse? CreateGetResponse(PaymentRecord payment)
    {
        if(payment == null)
            return null;

        return new GetPaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency ?? string.Empty,
            Amount = payment.Amount
        };
    }
}