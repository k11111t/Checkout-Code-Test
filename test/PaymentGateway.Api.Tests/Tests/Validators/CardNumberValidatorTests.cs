using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Validation;

namespace PaymentGateway.Api.Tests.Validators
{
    public class CardNumberValidatorTests
    {
        readonly CardNumberValidator validator;

        public CardNumberValidatorTests()
        {
            validator = new CardNumberValidator();
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenInputNull()
        {
            // arrange
            PostPaymentRequest request = null;

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCardNumberIsNull()
        {
            // Arrange
            var request = new PostPaymentRequest { CardNumber = null };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCardNumberTooShort()
        {
            // Arrange
            var request = new PostPaymentRequest { CardNumber = "123456789" };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCardNumberTooLong()
        {
            // Arrange
            var request = new PostPaymentRequest { CardNumber = "12345678901234567890" };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCardNumberContainsNonNumericals()
        {
            // Arrange
            var request = new PostPaymentRequest { CardNumber = "12345A7890123" };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenCardNumberIsValid()
        {
            // Arrange
            var request = new PostPaymentRequest { CardNumber = "1234567891111111" };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
