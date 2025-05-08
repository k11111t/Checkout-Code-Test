using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task GetPaymentAsync_ReturnsOkOnPaymentFound() {
        // Arrange
        var payment = new GetPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP",
            Status = Models.PaymentStatus.Authorized
        };

        var mockPaymentsManager = new Mock<IPaymentManager>();
        mockPaymentsManager.Setup(x => x.GetPaymentAsync(It.IsAny<Guid>()))
            .ReturnsAsync(payment);
            
        var paymentsController = new PaymentsController(mockPaymentsManager.Object);
        
        // Act
        ActionResult<GetPaymentResponse?> paymentConfirmationActionResult = await paymentsController.GetPaymentAsync(payment.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(paymentConfirmationActionResult.Result);
        var paymentResponse = Assert.IsType<GetPaymentResponse>(okResult.Value);
        Assert.Equal((int) HttpStatusCode.OK, okResult.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(payment.Id, paymentResponse.Id);
    }

    [Fact]
    public async Task GetPaymentAsync_Returns404OnPaymentNotFound() {
        // Arrange
        var mockPaymentsManager = new Mock<IPaymentManager>();
        mockPaymentsManager.Setup(x => x.GetPaymentAsync(It.IsAny<Guid>()))
            .ReturnsAsync((GetPaymentResponse?) null);
        var paymentsController = new PaymentsController(mockPaymentsManager.Object);
        Guid id = new();
        
        // Act
        ActionResult<GetPaymentResponse?> paymentConfirmationActionResult = await paymentsController.GetPaymentAsync(id);

        // Assert
        var result = Assert.IsType<NotFoundResult>(paymentConfirmationActionResult.Result);
        Assert.Equal((int) HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task PostPaymentAsync_ReturnsOkOnSuccessfulTransaction() {
        // Arrange
        var payment = new PostPaymentRequest
        {
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = "1234567890000000",
            Currency = "GBP",
            Cvv = 123
        };

        PostPaymentResponse expectedPaymentResponse = new()
        {
            Id = new(),
            ExpiryYear = payment.ExpiryYear,
            ExpiryMonth = payment.ExpiryMonth,
            Amount = payment.Amount,
            CardNumberLastFour = 0000,
            Currency = payment.Currency,
            Status = Models.PaymentStatus.Authorized
        };

        var mockPaymentsManager = new Mock<IPaymentManager>();
        mockPaymentsManager.Setup(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(expectedPaymentResponse);
        var paymentsController = new PaymentsController(mockPaymentsManager.Object);
        
        // Act
        ActionResult<PostPaymentResponse> paymentConfirmationActionResult = await paymentsController.PostPaymentAsync(payment);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(paymentConfirmationActionResult.Result);
        var paymentResponse = Assert.IsType<PostPaymentResponse>(okResult.Value);
        Assert.Equal((int) HttpStatusCode.OK, okResult.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(expectedPaymentResponse.Id, paymentResponse.Id);
        Assert.Equal(expectedPaymentResponse.ExpiryYear, paymentResponse.ExpiryYear);
        Assert.Equal(expectedPaymentResponse.ExpiryMonth, paymentResponse.ExpiryMonth);
        Assert.Equal(expectedPaymentResponse.Amount, paymentResponse.Amount);
        Assert.Equal(expectedPaymentResponse.CardNumberLastFour, paymentResponse.CardNumberLastFour);
        Assert.Equal(expectedPaymentResponse.Currency, paymentResponse.Currency);
        Assert.Equal(Models.PaymentStatus.Authorized, paymentResponse.Status);
    }

    [Fact]
    public async Task PostPaymentAsync_ReturnsBadRequestOnInvalidRequest()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = "123456789",
            Currency = "GBP",
            Cvv = 123
        };

        PostPaymentResponse expectedPaymentResponse = new()
            {
                Status = Models.PaymentStatus.Rejected
            };

        var mockPaymentsManager = new Mock<IPaymentManager>();
        mockPaymentsManager.Setup(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(expectedPaymentResponse);
        var paymentsController = new PaymentsController(mockPaymentsManager.Object);
        
        // Act
        ActionResult<PostPaymentResponse> paymentConfirmationActionResult = await paymentsController.PostPaymentAsync(payment);
        
        // Assert
        var result = Assert.IsType<BadRequestObjectResult>(paymentConfirmationActionResult.Result);
        var paymentResponse = Assert.IsType<PostPaymentResponse>(result.Value);
        Assert.Equal((int) HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(Models.PaymentStatus.Rejected, paymentResponse.Status);
    }
}