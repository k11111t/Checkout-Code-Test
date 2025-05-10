using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Interfaces;
public interface IPaymentRecordMapper
{
    public PaymentRecord? CreatePaymentRecord(PostPaymentRequest request);
}
    