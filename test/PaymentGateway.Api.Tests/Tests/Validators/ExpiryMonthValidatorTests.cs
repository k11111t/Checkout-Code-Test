using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.Tests
{
    public class ExpiryMonthValidatorTests
    {
        readonly ExpiryMonthValidator validator;

        public ExpiryMonthValidatorTests()
        {
            validator = new ExpiryMonthValidator();
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
            Assert.Contains("ExpiryMonth", errorDictionary.Keys);
            Assert.Equal("Expiry month must be between 1 and 12", errorDictionary["ExpiryMonth"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenExpiryMonthLessThanOne()
        {
            // Arrange
            var request = new PostPaymentRequest { ExpiryMonth = 0 };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("ExpiryMonth", errorDictionary.Keys);
            Assert.Equal("Expiry month must be between 1 and 12", errorDictionary["ExpiryMonth"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenExpiryMonthIsGreaterThanTwelve()
        {
            // Arrange
            var request = new PostPaymentRequest { ExpiryMonth = 13 };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("ExpiryMonth", errorDictionary.Keys);
            Assert.Equal("Expiry month must be between 1 and 12", errorDictionary["ExpiryMonth"]);
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenExpiryMonthValid()
        {
            // Arrange
            var request = new PostPaymentRequest { ExpiryMonth = 5 };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
