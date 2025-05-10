using Moq;
using Microsoft.Extensions.Logging;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Services.PaymentGateway;

namespace PaymentGateway.Api.Tests.Tests;
public class PaymentManagerTests
{
    readonly Mock<IValidatorManager<PostPaymentRequest>> validator = new();
    readonly Mock<IRepository<PaymentRecord>> repository = new();
    readonly Mock<IBankPaymentManager> bankManager = new();
    readonly Mock<ILogger<PaymentManager>> logger = new();

    PaymentManager CreatePaymentManager()
    {
        return new PaymentManager(
            repository.Object, 
            validator.Object, 
            bankManager.Object, 
            logger.Object
        );
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnAuthorized_OnValidationSuccess()
    {
        // Arrange
        var request = new PostPaymentRequest()
        {
            Amount = 123,
            CardNumber = "0000000000001234",
            Currency = "CZK",
            ExpiryMonth = 12,
            ExpiryYear = 1234,
        };

        var expectedResponse = new PostPaymentResponse()
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            CardNumberLastFour = 1234,
            Currency = request.Currency,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Status = PaymentStatus.Authorized
        };

        validator.Setup(x => x.Validate(request, out It.Ref<Dictionary<string,string>>.IsAny)).Returns(true);
        bankManager.Setup(x => x.ProcessBankPaymentAsync(It.IsAny<BankPaymentRequest>()))
                 .ReturnsAsync(new BankPaymentResponse { Authorized = true });
        var manager = CreatePaymentManager();

        // Act
        var result = await manager.ProcessPaymentAsync(request);

        // Assert
        Assert.Equal(PaymentStatus.Authorized, result.Status);
        Assert.Equal(expectedResponse.Amount, result.Amount);
        Assert.Equal(expectedResponse.CardNumberLastFour, result.CardNumberLastFour);
        Assert.Equal(expectedResponse.Currency, result.Currency);
        Assert.Equal(expectedResponse.ExpiryMonth, result.ExpiryMonth);
        Assert.Equal(expectedResponse.ExpiryYear, result.ExpiryYear);
    }
        
    [Fact]
    public async Task ProcessPaymentAsync_ReturnRejected_OnValidationFail()
    {
        // Arrange
        var request = new PostPaymentRequest();
        validator.Setup(x => x.Validate(request, out It.Ref<Dictionary<string,string>>.IsAny)).Returns(false);
        var manager = CreatePaymentManager();

        // Act
        var result = await manager.ProcessPaymentAsync(request);

        // Assert
        Assert.Equal(PaymentStatus.Rejected, result.Status);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsRejected_OnBankFail()
    {
        // Arrange
        var request = new PostPaymentRequest();
        validator.Setup(x => x.Validate(request, out It.Ref<Dictionary<string,string>>.IsAny)).Returns(true);
        bankManager.Setup(x => x.ProcessBankPaymentAsync(It.IsAny<BankPaymentRequest>()))
                 .ReturnsAsync((BankPaymentResponse?)null);
        var manager = CreatePaymentManager();

        // Act
        var result = await manager.ProcessPaymentAsync(request);

        // Assert
        Assert.Equal(PaymentStatus.Rejected, result.Status);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsDeclined_OnBankResponseNotAuthorized()
    {
        // Arrange
        var request = new PostPaymentRequest();
        validator.Setup(x => x.Validate(request, out It.Ref<Dictionary<string,string>>.IsAny)).Returns(true);
        bankManager.Setup(x => x.ProcessBankPaymentAsync(It.IsAny<BankPaymentRequest>()))
                 .ReturnsAsync(new BankPaymentResponse { Authorized = false });
        var manager = CreatePaymentManager();

        // Act
        var result = await manager.ProcessPaymentAsync(request);

        // Assert
        Assert.Equal(PaymentStatus.Declined, result.Status);
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsResponse_OnPaymentExists()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        repository.Setup(x => x.Get(expectedId)).Returns(new PaymentRecord { Id = expectedId });
        var manager = CreatePaymentManager();

        // Act
        var result = await manager.GetPaymentAsync(expectedId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id);
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsNull_OnPaymentMissing()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        repository.Setup(x => x.Get(expectedId)).Returns((PaymentRecord?)null);
        var manager = CreatePaymentManager();

        // Act
        var result = await manager.GetPaymentAsync(expectedId);

        // Assert
        Assert.Null(result);
    }
}
