using System.Text;
using System.Text.Json;

using PaymentGateway.Api.Models.Requests;

public class BankRequestBuilder : IBankRequestBuilder
{
    private readonly IPaymentGatewayConfiguration _config;
    public BankRequestBuilder(IPaymentGatewayConfiguration config)
    {
        _config = config;
    }
    public HttpRequestMessage BuildRequest(BankPaymentRequest request)
    {
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