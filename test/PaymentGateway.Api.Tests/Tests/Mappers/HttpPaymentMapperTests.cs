using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Services.Data;
using Xunit;

namespace PaymentGateway.Tests.Services.Data
{
    public class HttpPaymentMapperTests
    {
        private readonly HttpPaymentMapper mapper = new();

        [Fact]
        public void CreatePostResponse_ReturnsNull_WhenPaymentIsNull()
        {
            var result = mapper.CreatePostResponse(null);
            Assert.Null(result);
        }

        [Fact]
        public void CreatePostResponse_ReturnsMappedResponse_WhenPaymentIsValid()
        {
            var expectedPayment = new PaymentRecord
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = 1234,
                ExpiryMonth = 10,
                ExpiryYear = 2026,
                Currency = "USD",
                Amount = 1
            };

            var result = mapper.CreatePostResponse(expectedPayment);

            Assert.NotNull(result);
            Assert.Equal(expectedPayment.Id, result!.Id);
            Assert.Equal(expectedPayment.Status, result.Status);
            Assert.Equal(expectedPayment.CardNumberLastFour, result.CardNumberLastFour);
            Assert.Equal(expectedPayment.ExpiryMonth, result.ExpiryMonth);
            Assert.Equal(expectedPayment.ExpiryYear, result.ExpiryYear);
            Assert.Equal(expectedPayment.Currency, result.Currency);
            Assert.Equal(expectedPayment.Amount, result.Amount);
        }

        [Fact]
        public void CreatePostResponse_ReturnsEmptyCurrency_WhenCurrencyIsNull()
        {
            var payment = new PaymentRecord
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = 4321,
                ExpiryMonth = 5,
                ExpiryYear = 2027,
                Currency = null,
                Amount = 50
            };

            var result = mapper.CreatePostResponse(payment);

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Currency);
        }

        [Fact]
        public void CreateGetResponse_ReturnsNull_WhenPaymentIsNull()
        {
            var result = mapper.CreateGetResponse(null);
            Assert.Null(result);
        }

        [Fact]
        public void CreateGetResponse_ReturnsMappedResponse_WhenPaymentIsValid()
        {
            var expectedPayment = new PaymentRecord
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Declined,
                CardNumberLastFour = 5678,
                ExpiryMonth = 7,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 150
            };

            var result = mapper.CreateGetResponse(expectedPayment);

            Assert.NotNull(result);
            Assert.Equal(expectedPayment.Id, result.Id);
            Assert.Equal(expectedPayment.Status, result.Status);
            Assert.Equal(expectedPayment.CardNumberLastFour, result.CardNumberLastFour);
            Assert.Equal(expectedPayment.ExpiryMonth, result.ExpiryMonth);
            Assert.Equal(expectedPayment.ExpiryYear, result.ExpiryYear);
            Assert.Equal(expectedPayment.Currency, result.Currency);
            Assert.Equal(expectedPayment.Amount, result.Amount);
        }
    }
}
