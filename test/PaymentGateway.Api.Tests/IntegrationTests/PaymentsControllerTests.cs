using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Data;
using PaymentGateway.Api.Services.PaymentGateway;

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

        var paymentsManager = new Mock<IPaymentManager>();
        paymentsManager.Setup(x => x.GetPaymentAsync(It.IsAny<Guid>()))
            .ReturnsAsync(payment);
            
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => {
                services.AddSingleton(paymentsManager.Object);
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

        var paymentsManager = new Mock<IPaymentManager>();
        paymentsManager.Setup(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(expectedResponse);
            
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => {
                services.AddSingleton(paymentsManager.Object);
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

    [Fact]
    public async Task PostPaymentAsync_ReturnsOK_OnPaymentValid_Robust()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = "12341234111155511",
            Currency = "GBP"
        };

        var id = Guid.NewGuid();
        var expectedResponse = new PostPaymentResponse
        {
            Id = id,
            ExpiryYear = payment.ExpiryYear,
            ExpiryMonth = payment.ExpiryMonth,
            Amount = payment.Amount,
            Currency = payment.Currency,
            CardNumberLastFour = 5511,
            Status = Models.PaymentStatus.Authorized
        };

        // Mock dependencies
        var repository = new Mock<IRepository<PaymentRecord>>();
        var validator = new Mock<IValidatorManager<PostPaymentRequest>>();
        var bankManager = new Mock<IBankPaymentManager>();
        var bankMapper = new Mock<IBankPaymentMapper>();
        var paymentRecordMapper = new Mock<IPaymentRecordMapper>();
        var httpMapper = new Mock<IHttpPaymentMapper>();
        var logger = new Mock<ILogger<PaymentManager>>();

        validator.Setup(x => x.Validate(It.IsAny<PostPaymentRequest>(), out It.Ref<Dictionary<string,string>>.IsAny))
            .Returns(true);

        bankManager.Setup(x => x.ProcessBankPaymentAsync(It.IsAny<BankPaymentRequest>()))
            .ReturnsAsync(new BankPaymentResponse { Authorized = true });

        bankMapper.Setup(x => x.CreateBankPaymentRequest(It.IsAny<PostPaymentRequest>()))
            .Returns(new BankPaymentRequest(){
                CardNumber = "",
                Amount = 1,
                Currency = "",
                Cvv = "",
                ExpiryDate = ""
            });
        
        paymentRecordMapper.Setup(x => x.CreatePaymentRecord(It.IsAny<PostPaymentRequest>()))
            .Returns(new PaymentRecord());
        
        httpMapper.Setup(x => x.CreatePostResponse(It.IsAny<PaymentRecord>()))
            .Returns(expectedResponse);

        var webApplicationFactory = new WebApplicationFactory<Program>();
            
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => {
                services.AddSingleton(repository.Object);
                services.AddSingleton(validator.Object);
                services.AddSingleton(bankManager.Object);
                services.AddSingleton(bankMapper.Object);
                services.AddSingleton(httpMapper.Object);
                services.AddSingleton(paymentRecordMapper.Object);
                services.AddSingleton(logger.Object);
                services.AddSingleton<IPaymentManager, PaymentManager>();
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
        Assert.Equal(id, actualResponse.Id);
        Assert.Equal(expectedResponse.Currency, actualResponse.Currency);
        Assert.Equal(expectedResponse.Amount, actualResponse.Amount);
        Assert.Equal(expectedResponse.ExpiryYear, actualResponse.ExpiryYear);
        Assert.Equal(expectedResponse.ExpiryMonth, actualResponse.ExpiryMonth);
        Assert.Equal(expectedResponse.CardNumberLastFour, actualResponse.CardNumberLastFour);
        Assert.Equal(expectedResponse.Status, actualResponse.Status);
    }

}