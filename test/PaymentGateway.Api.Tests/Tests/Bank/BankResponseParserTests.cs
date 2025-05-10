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
    public async Task ParseResponseAsync_ReturnsValidResponse_OnValidTrueInput()
    {
        // Arrange
        var expectedResponse = new BankPaymentResponse(){
            Authorized = true,
            AuthorizationCode = "1234"
        };

        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
        };

        // Act
        var result = await bankResponseParser.ParseResponseAsync(httpResponse);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.AuthorizationCode, result.AuthorizationCode);
        Assert.Equal(expectedResponse.Authorized, result.Authorized);
    }

    [Fact]
    public async Task ParseResponseAsync_ReturnsValidResponse_OnValidFalseInput()
    {
        // Arrange
        var expectedResponse = new BankPaymentResponse(){
            Authorized = false,
            AuthorizationCode = "1234"
        };

        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
        };

        // Act
        var result = await bankResponseParser.ParseResponseAsync(httpResponse);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.AuthorizationCode, result.AuthorizationCode);
        Assert.Equal(expectedResponse.Authorized, result.Authorized);
    }

    [Fact]
    public async Task ParseResponseAsync_ReturnsNull_OnInvalidContent()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent("jibberish{}")
        };

        // Act
        var result = await bankResponseParser.ParseResponseAsync(httpResponse);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task ParseResponseAsync_ReturnsNull_OnInvalidInput()
    {
        // Arrange

        // Act
        var result = await bankResponseParser.ParseResponseAsync(null);

        // Assert
        Assert.Null(result);
    }
}