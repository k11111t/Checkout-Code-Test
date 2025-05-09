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
        return new PaymentManager(repository.Object, validator.Object, bankManager.Object, logger.Object);
    }
        
    [Fact]
    public async Task ProcessPaymentAsync_ReturnRejectedOnValidationFail()
    {
        // arrange
        var request = new PostPaymentRequest();
        validator.Setup(x => x.Validate(request, out It.Ref<Dictionary<string,string>>.IsAny)).Returns(false);
        var manager = CreatePaymentManager();

        // act
        var result = await manager.ProcessPaymentAsync(request);

        // assert
        Assert.Equal(PaymentStatus.Rejected, result.Status);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsRejectedOnBankFail()
    {
        // arrange
        var request = new PostPaymentRequest();
        validator.Setup(x => x.Validate(request, out It.Ref<Dictionary<string,string>>.IsAny)).Returns(true);
        bankManager.Setup(x => x.ProcessBankPaymentAsync(It.IsAny<BankPaymentRequest>()))
                 .ReturnsAsync((BankPaymentResponse?)null);
        var manager = CreatePaymentManager();

        // act
        var result = await manager.ProcessPaymentAsync(request);

        // assert
        Assert.Equal(PaymentStatus.Rejected, result.Status);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsDeclinedOnBankResponseNotAuthorized()
    {
        var request = new PostPaymentRequest();
        validator.Setup(x => x.Validate(request, out It.Ref<Dictionary<string,string>>.IsAny)).Returns(true);
        bankManager.Setup(x => x.ProcessBankPaymentAsync(It.IsAny<BankPaymentRequest>()))
                 .ReturnsAsync(new BankPaymentResponse { Authorized = false });

        var manager = CreatePaymentManager();

        var result = await manager.ProcessPaymentAsync(request);

        Assert.Equal(PaymentStatus.Declined, result.Status);
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsResponseOnPaymentExists()
    {
        var expectedId = Guid.NewGuid();
        repository.Setup(x => x.Get(expectedId)).Returns(new PaymentRecord { Id = expectedId });

        var manager = CreatePaymentManager();

        var result = await manager.GetPaymentAsync(expectedId);

        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id);
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsNullOnPaymentMissing()
    {
        var expectedId = Guid.NewGuid();
        repository.Setup(x => x.Get(expectedId)).Returns((PaymentRecord?)null);

        var manager = CreatePaymentManager();

        var result = await manager.GetPaymentAsync(expectedId);

        Assert.Null(result);
    }
}
