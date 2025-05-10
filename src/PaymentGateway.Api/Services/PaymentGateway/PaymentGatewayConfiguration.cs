namespace PaymentGateway.Api.Services.PaymentGateway;

public class PaymentGatewayConfiguration : IPaymentGatewayConfiguration
{
    public List<string> SupportedCurrencies { get; set; } = new();
    public string BankServiceUrl { get; set; } = string.Empty;
}