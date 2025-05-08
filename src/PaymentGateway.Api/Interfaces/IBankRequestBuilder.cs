using PaymentGateway.Api.Models.Requests;

public interface IBankRequestBuilder
{
    public HttpRequestMessage BuildRequest(BankPaymentRequest request);
}
