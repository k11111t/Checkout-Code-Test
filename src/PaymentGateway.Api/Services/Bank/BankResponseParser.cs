using System.Text.Json;

using PaymentGateway.Api.Models.Responses;

public class BankResponseParser : IBankResponseParser
{
    public async Task<BankPaymentResponse?> ParseResponseAsync(HttpResponseMessage httpResponse)
    {
        Stream responseStream = await httpResponse.Content.ReadAsStreamAsync();
        BankPaymentResponse? bankResponseObj = await JsonSerializer.DeserializeAsync<BankPaymentResponse>(responseStream);

        return bankResponseObj;
    }
}