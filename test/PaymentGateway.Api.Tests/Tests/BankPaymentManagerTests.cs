using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Bank;


namespace PaymentGateway.Api.Tests.Tests;
public class BankPaymentManagerTest
{
    readonly Mock<HttpMessageHandler> httpHandler = new();
    readonly Mock<ILogger<BankPaymentManager>> logger = new();
    readonly Mock<IBankRequestBuilder> requestBuilder = new();
    readonly Mock<IBankResponseParser> responseParser = new();

    readonly BankPaymentRequest DummyRequest = new(){
        CardNumber = "DUMMY",
        Amount = 1,
        Currency = "DUMMY",
        Cvv = "DUMMY",
        ExpiryDate = "DUMMY"
    };

    [Fact]
    public async Task ProcessBankPaymentAsync_ReturnsBankResponse_OnValidRequest()
    {
        // Arrange
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://testuri.com");
        var httpResponse = new HttpResponseMessage(){
            StatusCode = HttpStatusCode.OK
        };

        httpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var client = new Mock<HttpClient>(httpHandler.Object);

        var expectedResponse = new BankPaymentResponse(){
            AuthorizationCode = "123456789",
            Authorized = true
        };

        requestBuilder.Setup(x => x.BuildRequest(It.IsAny<BankPaymentRequest>())).Returns(httpRequest);
        responseParser.Setup(x => x.ParseResponseAsync(It.IsAny<HttpResponseMessage>())).ReturnsAsync(expectedResponse);

        BankPaymentManager bankManager = new
        (
            client.Object, 
            logger.Object,
            requestBuilder.Object,
            responseParser.Object
        );

        // Act
        var result =  await bankManager.ProcessBankPaymentAsync(DummyRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Authorized, result.Authorized);
        Assert.Equal(expectedResponse.AuthorizationCode, result.AuthorizationCode);
    }

    [Fact]
    public async Task ProcessBankPaymentAsync_ReturnsNull_WhenRequestNull()
    {
        // Arrange
        var client = new Mock<HttpClient>(httpHandler.Object);

        BankPaymentManager bankManager = new
        (
            client.Object, 
            logger.Object,
            requestBuilder.Object,
            responseParser.Object
        );

        // Act
        var result =  await bankManager.ProcessBankPaymentAsync(DummyRequest);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ProcessBankPaymentAsync_ReturnsNull_WhenRequestBuilderReturnsNull()
    {
        // Arrange
        var client = new Mock<HttpClient>(httpHandler.Object);

        requestBuilder.Setup(x => x.BuildRequest(It.IsAny<BankPaymentRequest>())).Returns((HttpRequestMessage?)null);

        BankPaymentManager bankManager = new
        (
            client.Object, 
            logger.Object,
            requestBuilder.Object,
            responseParser.Object
        );

        // Act
        var result =  await bankManager.ProcessBankPaymentAsync(DummyRequest);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ProcessBankPaymentAsync_ReturnsNull_WhenRequestBuilderReturnsInvalidHttpRequest()
    {
        // Arrange
        var client = new Mock<HttpClient>(httpHandler.Object);

        requestBuilder.Setup(x => x.BuildRequest(It.IsAny<BankPaymentRequest>())).Returns(new HttpRequestMessage());

        BankPaymentManager bankManager = new
        (
            client.Object, 
            logger.Object,
            requestBuilder.Object,
            responseParser.Object
        );

        // Act
        var result =  await bankManager.ProcessBankPaymentAsync(DummyRequest);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ProcessBankPaymentAsync_ReturnsNull_OnServiceUnavailableHttpResponse()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

        httpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var client = new Mock<HttpClient>(httpHandler.Object);

        requestBuilder.Setup(x => x.BuildRequest(It.IsAny<BankPaymentRequest>())).Returns(new HttpRequestMessage());

        BankPaymentManager bankManager = new
        (
            client.Object, 
            logger.Object,
            requestBuilder.Object,
            responseParser.Object
        );

        // Act
        var result =  await bankManager.ProcessBankPaymentAsync(DummyRequest);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ProcessBankPaymentAsync_ReturnsDeclinedPayment_OnBadRequestHttpResponse()
    {
        // Arrange
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://testuri.com");
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

        httpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var client = new Mock<HttpClient>(httpHandler.Object);

        requestBuilder.Setup(x => x.BuildRequest(It.IsAny<BankPaymentRequest>())).Returns(httpRequest);

        BankPaymentManager bankManager = new
        (
            client.Object, 
            logger.Object,
            requestBuilder.Object,
            responseParser.Object
        );

        // Act
        var result =  await bankManager.ProcessBankPaymentAsync(DummyRequest);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Authorized);
        Assert.Empty(result.AuthorizationCode);
    }

    [Fact]
    public async Task ProcessBankPaymentAsync_ReturnsNull_OnRequestParserReturnsNull()
    {
        // Arrange
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://testuri.com");

        var httpResponse = new HttpResponseMessage(){
            StatusCode = HttpStatusCode.OK
        };

        httpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        var client = new Mock<HttpClient>(httpHandler.Object);

        var expectedResponse = new BankPaymentResponse(){
            AuthorizationCode = "123456789",
            Authorized = true
        };

        requestBuilder.Setup(x => x.BuildRequest(It.IsAny<BankPaymentRequest>())).Returns(httpRequest);
        responseParser.Setup(x => x.ParseResponseAsync(It.IsAny<HttpResponseMessage>())).ReturnsAsync((BankPaymentResponse?) null);

        BankPaymentManager bankManager = new
        (
            client.Object, 
            logger.Object,
            requestBuilder.Object,
            responseParser.Object
        );

        // Act
        var result =  await bankManager.ProcessBankPaymentAsync(DummyRequest);

        // Assert
        Assert.Null(result);
    }
}