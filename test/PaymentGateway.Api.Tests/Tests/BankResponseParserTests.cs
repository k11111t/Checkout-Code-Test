using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.Tests;
public class BankResponseParserTests
{
    readonly Mock<ILogger<BankResponseParser>> logger = new ();

    readonly BankResponseParser bankResponseParser;

    public BankResponseParserTests()
    {
        bankResponseParser = new BankResponseParser(logger.Object);
    }

    [Fact]
    public async Task ParseResponseAsync_ReturnsValidResponseOnValidInput()
    {
        // arrange
        var expectedResponse = new BankPaymentResponse(){
            Authorized = true,
            AuthorizationCode = "1234"
        };

        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
        };

        // act
        var result = await bankResponseParser.ParseResponseAsync(httpResponse);

        // assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.AuthorizationCode, result.AuthorizationCode);
        Assert.Equal(expectedResponse.Authorized, result.Authorized);
    }

    [Fact]
    public async Task ParseResponseAsync_ReturnsNullOnInvalidContent()
    {
        // arrange
        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent("jibberish{}")
        };

        // act
        var result = await bankResponseParser.ParseResponseAsync(httpResponse);

        // assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task ParseResponseAsync_ReturnsNullOnInvalidInput()
    {
        // arrange

        // act
        var result = await bankResponseParser.ParseResponseAsync(null);

        // assert
        Assert.Null(result);
    }
}