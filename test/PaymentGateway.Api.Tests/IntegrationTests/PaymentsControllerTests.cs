using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.IntegrationTests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task GetPaymentAsync_RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new GetPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP"
        };

        var mockPaymentsManager = new Mock<IPaymentManager>();
        mockPaymentsManager.Setup(x => x.GetPaymentAsync(It.IsAny<Guid>()))
            .ReturnsAsync(payment);
            
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => {
                services.AddSingleton(mockPaymentsManager.Object);
            })).CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(payment.Id, paymentResponse.Id);
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsNotFound_WhenPaymentMissing()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task PostPaymentAsync_ReturnsOK_OnPaymentValid()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = "1234",
            Currency = "GBP"
        };

        var expectedResponse = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = 1234,
            ExpiryMonth = 1234,
            Amount = 1234,
            Currency = "GBP",
            CardNumberLastFour = 1234,
            Status = Models.PaymentStatus.Authorized
        };

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();

        var mockPaymentsManager = new Mock<IPaymentManager>();
        mockPaymentsManager.Setup(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(expectedResponse);
            
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => {
                services.AddSingleton(mockPaymentsManager.Object);
            })).CreateClient();
        
        StringContent content = new StringContent(
            JsonSerializer.Serialize(payment), 
            Encoding.UTF8, 
            "application/json"
        );
    
        HttpRequestMessage httpRequest = new(HttpMethod.Post, "/api/Payments/")
        {
            Content = content
        };
        
        // Act
        var response = await client.SendAsync(httpRequest);
        var actualResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);
        Assert.Equal(expectedResponse.Id, actualResponse.Id);
        Assert.Equal(expectedResponse.Currency, actualResponse.Currency);
        Assert.Equal(expectedResponse.Amount, actualResponse.Amount);
        Assert.Equal(expectedResponse.ExpiryYear, actualResponse.ExpiryYear);
        Assert.Equal(expectedResponse.ExpiryMonth, actualResponse.ExpiryMonth);
        Assert.Equal(expectedResponse.CardNumberLastFour, actualResponse.CardNumberLastFour);
        Assert.Equal(expectedResponse.Status, actualResponse.Status);
    }

}