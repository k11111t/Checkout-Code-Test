using System.Text.Json;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.Bank;
public class BankPaymentManager : IBankPaymentManager {
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankPaymentManager> _logger;
    private readonly IBankRequestBuilder _requestBuilder;
    private readonly IBankResponseParser _responseParser;

    public BankPaymentManager(
        HttpClient client, 
        ILogger<BankPaymentManager> logger,
        IBankRequestBuilder requestBuilder,
        IBankResponseParser responseParser)
    {
        _httpClient = client;
        _logger = logger;
        _requestBuilder = requestBuilder;
        _responseParser = responseParser;
    }
    
    public async Task<BankPaymentResponse?> ProcessBankPaymentAsync(BankPaymentRequest request)
    {
        try
        {
            if (request == null)
            {
                _logger.LogError("Request cannot be null");
                return null;
            }

            // build request
            HttpRequestMessage? bankRequestMessage = _requestBuilder.BuildRequest(request);
            if(bankRequestMessage == null)
            {
                _logger.LogError("Failed to create http request");
                return null;
            }

            // send request to the bank
            HttpResponseMessage bankResponseMessage = await _httpClient.SendAsync(bankRequestMessage);
            
            if(bankResponseMessage.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) 
            {
                _logger.LogError("Bank service is unavailable");
                return null;
            }
            if(bankResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest) 
            {
                _logger.LogWarning("Bank failed to authorise payment");
                return new BankPaymentResponse() {
                    AuthorizationCode = "",
                    Authorized = false
                };
            }

            // parse the response
            BankPaymentResponse? bankPaymentResponse = await _responseParser.ParseResponseAsync(bankResponseMessage);
            
            if(bankPaymentResponse == null)
            {
                _logger.LogError("Failed to parse bank response.");
                return null;
            }

            return bankPaymentResponse;
        }
        catch (HttpRequestException ex) 
        {
            _logger.LogError(ex, "Failed to connect to the bank service");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize bank response");
            return null;
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Bank failed to process payment");
            return null;
        }        
    }
}