using System.Text;
using System.Text.Json;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.Bank;
public class BankPaymentManager : IBankPaymentManager {
    private readonly IPaymentGatewayConfiguration _configuration;

    public BankPaymentManager(IPaymentGatewayConfiguration configuration){
        _configuration = configuration;
    }
    
    public async Task<BankPaymentResponse> ProcessBankPaymentAsync(BankPaymentRequest request) {
        string json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");   

        HttpClient httpClient= new HttpClient();
        HttpResponseMessage bankResponse = await httpClient.PostAsync(_configuration.BankServiceUrl, content);

        Stream responseStream = await bankResponse.Content.ReadAsStreamAsync();
        BankPaymentResponse? bankResponseObj = await JsonSerializer.DeserializeAsync<BankPaymentResponse>(responseStream);
        bankResponse.EnsureSuccessStatusCode();
        return bankResponseObj;
    }
}