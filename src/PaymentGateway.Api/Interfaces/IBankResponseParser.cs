using PaymentGateway.Api.Models.Responses;

public interface IBankResponseParser
{
    public Task<BankPaymentResponse?> ParseResponseAsync(HttpResponseMessage httpResponse);
}