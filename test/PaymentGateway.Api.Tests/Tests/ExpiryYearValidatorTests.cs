using PaymentGateway.Api.Models.Requests;
using Moq;
using System;

namespace PaymentGateway.Api.Tests.Tests
{
    public class ExpiryYearValidatorTests
    {
        readonly ExpiryYearValidator validator;

        public ExpiryYearValidatorTests()
        {
            validator = new ExpiryYearValidator();
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenExpiryYearInThePast()
        {
            // Arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year - 1 };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("ExpiryYear", errorDictionary.Keys);
            Assert.Equal("Expiry year must be in the future", errorDictionary["ExpiryYear"]);
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenExpiryYearInTheFuture()
        {
            // Arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year + 1 };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
