using PaymentGateway.Api.Services.PaymentGateway;

namespace PaymentGateway.Api.Tests.Tests;
public class PaymentGatewayConfigurationTests
{
    [Fact]
    public void Config_ReturnEmptyConfigOnCreate()
    {
        //act
        PaymentGatewayConfiguration paymentGatewayConfiguration = new();

        //assert
        Assert.NotNull(paymentGatewayConfiguration.BankServiceUrl);
        Assert.NotNull(paymentGatewayConfiguration.SupportedCurrencies);
    }

    [Fact]
    public void Config_ReturnCorrectValuesOnAssignment()
    {
        //assert
        PaymentGatewayConfiguration expected = new() {
            BankServiceUrl = "url",
            SupportedCurrencies = ["EUR", "CZK"]
        };
        PaymentGatewayConfiguration paymentGatewayConfiguration = new();

        //act
        paymentGatewayConfiguration.BankServiceUrl = expected.BankServiceUrl;
        paymentGatewayConfiguration.SupportedCurrencies = expected.SupportedCurrencies;

        //assert
        Assert.Equal(expected.BankServiceUrl, paymentGatewayConfiguration.BankServiceUrl);
        Assert.Equal(expected.SupportedCurrencies, paymentGatewayConfiguration.SupportedCurrencies);
    }

}