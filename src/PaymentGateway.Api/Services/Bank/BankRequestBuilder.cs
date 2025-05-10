using System.Text;
using System.Text.Json;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Bank;
public class BankRequestBuilder : IBankRequestBuilder
{
    private readonly IPaymentGatewayConfiguration _config;
    private readonly ILogger<BankRequestBuilder>  _logger;
    public BankRequestBuilder(IPaymentGatewayConfiguration config, ILogger<BankRequestBuilder> logger)
    {
        _config = config;
        _logger = logger;
    }
    public HttpRequestMessage? BuildRequest(BankPaymentRequest request)
    {
        if(request == null)
        {
            _logger.LogWarning("Unable to build request, argument is null");
            return null;
        }

        StringContent content = new StringContent(
            JsonSerializer.Serialize(request), 
            Encoding.UTF8, 
            "application/json"
        );
    
        HttpRequestMessage httpRequest = new(HttpMethod.Post, _config.BankServiceUrl)
        {
            Content = content
        };
    
        return httpRequest;
        
    }
}