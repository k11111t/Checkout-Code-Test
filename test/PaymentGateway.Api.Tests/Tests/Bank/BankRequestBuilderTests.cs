using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.Tests;
public class BankRequestBuilderTests
{
    readonly Mock<IPaymentGatewayConfiguration> config = new ();
    readonly Mock<ILogger<BankRequestBuilder>> logger = new ();

    BankRequestBuilder CreateBankRequestBuilder()
    {
        return new BankRequestBuilder(config.Object, logger.Object);
    }

    [Fact]
    public async Task BuildRequest_ReturnsValidRequest_OnValidInput()
    {
        // arrange
        var request = new BankPaymentRequest(){
            CardNumber = "DUMMY",
            Amount = 1,
            Currency = "DUMMY",
            Cvv = "DUMMY",
            ExpiryDate = "03/2024"
        };
        string url = "http://dummy_test.com";
        config.Setup(x => x.BankServiceUrl).Returns(url);

        var bankRequestBuilder = CreateBankRequestBuilder();

        // act
        var result = bankRequestBuilder.BuildRequest(request);

        // assert
        Assert.NotNull(result);
        Assert.Equal(HttpMethod.Post, result.Method);
        Assert.Contains(url, result.RequestUri.ToString());
        var requestBody = await result.Content.ReadAsStringAsync();
        Assert.Contains(request.ExpiryDate, requestBody);
    }

    [Fact]
    public void BuildRequest_ReturnsNull_OnNullInput()
    {
        //assert
        var bankRequestBuilder = CreateBankRequestBuilder();
        // act
        var result = bankRequestBuilder.BuildRequest(null);

        // assert
        Assert.Null(result);
        
    }
}