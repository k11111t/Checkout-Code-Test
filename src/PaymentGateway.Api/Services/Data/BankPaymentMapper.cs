using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Data;
public class BankPaymentMapper : IBankPaymentMapper
{
    public BankPaymentRequest? CreateBankPaymentRequest(PostPaymentRequest request)
    {
        if(request == null)
            return null;

        return new BankPaymentRequest()
        {
            CardNumber = request.CardNumber ?? string.Empty,
            ExpiryDate =  $"{request.ExpiryMonth}/{request.ExpiryYear}",
            Currency = request.Currency ?? string.Empty,
            Amount = request.Amount,
            Cvv = request.Cvv.ToString("D3")
        };
    }
}