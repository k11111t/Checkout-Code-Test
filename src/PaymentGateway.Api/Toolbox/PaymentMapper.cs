using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Toolbox;
public static class PaymentMapper
{
    public static PostPaymentResponse CreatePostResponse(PaymentRecord payment)
    {
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

    public static GetPaymentResponse CreateGetResponse(PaymentRecord payment)
    {
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

    public static BankPaymentRequest CreateBankPaymentRequest(PostPaymentRequest request)
    {
        return new BankPaymentRequest()
        {
            CardNumber = request.CardNumber ?? string.Empty,
            ExpiryDate =  $"{request.ExpiryMonth}/{request.ExpiryYear}",
            Currency = request.Currency ?? string.Empty,
            Amount = request.Amount,
            Cvv = request.Cvv.ToString()
        };
    }

    public static PaymentRecord CreatePaymentRecord(PostPaymentRequest request)
    {
        string cardNumber = request.CardNumber ?? string.Empty;
        int lastFour = 0;
        if (cardNumber.Length >= 4)
            lastFour = int.TryParse(cardNumber.Substring(cardNumber.Length-4, 4), out int parsed) ? parsed : 0;
        
        return new PaymentRecord()
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = lastFour,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency ?? string.Empty,
            Amount = request.Amount
        };
    }
}