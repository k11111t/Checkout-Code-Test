using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces;
public interface IBankPaymentMapper
{
    public BankPaymentRequest? CreateBankPaymentRequest(PostPaymentRequest request);
}