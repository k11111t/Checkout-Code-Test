using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Data;

namespace PaymentGateway.Api.Tests.Mappers
{
    public class BankPaymentMapperTests
    {
        readonly BankPaymentMapper mapper = new();

        public BankPaymentMapperTests()
        {
            mapper = new();
        }

        [Fact]
        public void CreateBankPaymentRequest_ReturnsNull_WhenRequestIsNull()
        {
            //Arrange

            // Act
            var result = mapper.CreateBankPaymentRequest(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CreateBankPaymentRequest_ReturnsValidObject_WhenRequestIsValid()
        {
            // Arrange
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567890111111",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = "EUR",
                Amount = 100,
                Cvv = 123
            };

            // Act
            var result = mapper.CreateBankPaymentRequest(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1234567890111111", result.CardNumber);
            Assert.Equal("12/2025", result.ExpiryDate);
            Assert.Equal("EUR", result.Currency);
            Assert.Equal(100, result.Amount);
            Assert.Equal("123", result.Cvv);
        }

        [Fact]
        public void CreateBankPaymentRequest_ReturnsValidObject_WhenRequestIsValid_PaddedCvv()
        {
            // Arrange
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567890123456",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = "EUR",
                Amount = 100,
                Cvv = 1
            };

            // Act
            var result = mapper.CreateBankPaymentRequest(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1234567890123456", result.CardNumber);
            Assert.Equal("12/2025", result.ExpiryDate);
            Assert.Equal("EUR", result.Currency);
            Assert.Equal(100, result.Amount);
            Assert.Equal("001", result.Cvv);
        }

        [Fact]
        public void CreateBankPaymentRequest_ReturnsNonNullValues_WhenEntriesInRequestAreNull()
        {
            // Arrange
            var request = new PostPaymentRequest
            {
                CardNumber = null,
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = null,
                Amount = 10,
                Cvv = 7
            };

            // Act
            var result = mapper.CreateBankPaymentRequest(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result!.CardNumber);
            Assert.Equal("1/2026", result.ExpiryDate);
            Assert.Equal(string.Empty, result.Currency);
            Assert.Equal("007", result.Cvv);
        }
    }
}
