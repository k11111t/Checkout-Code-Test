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
        public void Validate_ReturnsFalseWhenCurrencyNotSupported()
        {
            // arrange
            var request = new PostPaymentRequest { Currency = "CZK" };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("Currency", errorDictionary.Keys);
            Assert.Equal("Currency CZK is not supported.", errorDictionary["Currency"]);
        }

        [Fact]
        public void Validate_ReturnsTrueWhenCurrencySupported()
        {
            // arrange
            var request = new PostPaymentRequest { Currency = "USD" };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }

        [Fact]
        public void Validate_ReturnsFalseWhenCurrencyNull()
        {
            // arrange
            var request = new PostPaymentRequest { Currency = null };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("Currency", errorDictionary.Keys);
            Assert.Equal("Currency is not supported.", errorDictionary["Currency"]);
        }
    }
}
