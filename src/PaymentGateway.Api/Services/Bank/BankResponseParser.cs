using System.Text.Json;

using PaymentGateway.Api.Models.Responses;

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
                
            Stream responseStream = await httpResponse.Content.ReadAsStreamAsync();
            BankPaymentResponse? bankResponseObj = await JsonSerializer.DeserializeAsync<BankPaymentResponse>(responseStream);

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