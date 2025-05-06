public interface IPaymentGatewayConfiguration {
    List<string> SupportedCurrencies { get; }
    string BankServiceUrl { get; }
}