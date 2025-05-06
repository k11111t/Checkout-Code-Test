using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces
{
    public interface IBankPaymentManager
    {
        Task<BankPaymentResponse> ProcessBankPaymentAsync(BankPaymentRequest request);
    }
}