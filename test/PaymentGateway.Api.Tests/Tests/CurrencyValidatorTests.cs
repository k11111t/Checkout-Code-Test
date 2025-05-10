using PaymentGateway.Api.Models.Requests;
using Moq;

namespace PaymentGateway.Api.Tests.Tests
{
    public class CurrencyValidatorTests
    {
        readonly CurrencyValidator validator;
        readonly Mock<IPaymentGatewayConfiguration> config;

        public CurrencyValidatorTests()
        {
            config = new Mock<IPaymentGatewayConfiguration>();
            config.Setup(c => c.SupportedCurrencies).Returns(new List<string> { "USD", "EUR", "GBP" });
            validator = new CurrencyValidator(config.Object);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCurrencyNotSupported()
        {
            // Arrange
            var request = new PostPaymentRequest { Currency = "CZK" };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("Currency", errorDictionary.Keys);
            Assert.Equal("Currency CZK is not supported.", errorDictionary["Currency"]);
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenCurrencySupported()
        {
            // Arrange
            var request = new PostPaymentRequest { Currency = "USD" };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCurrencyNull()
        {
            // Arrange
            var request = new PostPaymentRequest { Currency = null };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("Currency", errorDictionary.Keys);
            Assert.Equal("Currency  is not supported.", errorDictionary["Currency"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCurrencyEmpty()
        {
            // Arrange
            var request = new PostPaymentRequest { Currency = string.Empty };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("Currency", errorDictionary.Keys);
            Assert.Equal("Currency  is not supported.", errorDictionary["Currency"]);
        }
    }
}
