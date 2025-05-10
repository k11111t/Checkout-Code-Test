using System.Text.Json;

using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.Bank;
public class BankResponseParser : IBankResponseParser
{
    private readonly ILogger<BankResponseParser>  _logger;

    public BankResponseParser(ILogger<BankResponseParser> logger)    
    {
        _logger = logger;
    }

    public async Task<BankPaymentResponse?> ParseResponseAsync(HttpResponseMessage httpResponse)
    {
        try
        {
            if (httpResponse?.Content == null)
                return null;
                
            BankPaymentResponse? bankResponseObj = await httpResponse.Content.ReadFromJsonAsync<BankPaymentResponse>();

            return bankResponseObj;
        }
        catch(JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialise response");
            return null;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialise response");
            return null;
        }
        
    }
}