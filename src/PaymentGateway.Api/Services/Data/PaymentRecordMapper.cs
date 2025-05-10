using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Data;
public class PaymentRecordMapper : IPaymentRecordMapper
{
    public PaymentRecord? CreatePaymentRecord(PostPaymentRequest request)
    {
        if(request == null)
            return null;

        string cardNumber = request.CardNumber ?? string.Empty;
        int lastFour = 0;
        if (cardNumber.Length >= 4)
        {
            if(cardNumber.All(x => char.IsDigit(x)))
                lastFour = int.TryParse(cardNumber.Substring(cardNumber.Length-4, 4), out int parsed) ? parsed : 0;
            else
                lastFour = 0;
        }
            
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
    