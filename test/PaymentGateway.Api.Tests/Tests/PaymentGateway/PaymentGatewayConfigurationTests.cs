using PaymentGateway.Api.Services.PaymentGateway;

namespace PaymentGateway.Api.Tests.PaymentGateway;
public class PaymentGatewayConfigurationTests
{
    [Fact]
    public void Config_ReturnEmptyConfigOnCreate()
    {
        // Act
        PaymentGatewayConfiguration paymentGatewayConfiguration = new();

        // Assert
        Assert.NotNull(paymentGatewayConfiguration.BankServiceUrl);
        Assert.NotNull(paymentGatewayConfiguration.SupportedCurrencies);
    }

    [Fact]
    public void Config_ReturnCorrectValuesOnAssignment()
    {
        // Assert
        PaymentGatewayConfiguration expected = new() {
            BankServiceUrl = "url",
            SupportedCurrencies = ["EUR", "CZK"]
        };
        PaymentGatewayConfiguration paymentGatewayConfiguration = new();

        // Act
        paymentGatewayConfiguration.BankServiceUrl = expected.BankServiceUrl;
        paymentGatewayConfiguration.SupportedCurrencies = expected.SupportedCurrencies;

        // Assert
        Assert.Equal(expected.BankServiceUrl, paymentGatewayConfiguration.BankServiceUrl);
        Assert.Equal(expected.SupportedCurrencies, paymentGatewayConfiguration.SupportedCurrencies);
    }

}