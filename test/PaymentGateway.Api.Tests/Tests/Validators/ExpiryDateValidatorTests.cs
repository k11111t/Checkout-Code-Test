using PaymentGateway.Api.Models.Requests;
using Moq;
using System;

namespace PaymentGateway.Api.Tests.Tests
{
    public class ExpiryDateValidatorTests
    {
        readonly ExpiryDateValidator validator;

        public ExpiryDateValidatorTests()
        {
            validator = new ExpiryDateValidator();
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
            Assert.Contains("ExpiryYear", errorDictionary.Keys);
            Assert.Equal("Expiry year must be in the future", errorDictionary["ExpiryYear"]);
            Assert.Contains("ExpiryMonth", errorDictionary.Keys);
            Assert.Equal("Expiry month must be in the future", errorDictionary["ExpiryMonth"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenExpiryYearInThePast()
        {
            // Arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year - 1, ExpiryMonth = DateTime.Now.Month };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("ExpiryYear", errorDictionary.Keys);
            Assert.Equal("Expiry year must be in the future", errorDictionary["ExpiryYear"]);
            Assert.Contains("ExpiryMonth", errorDictionary.Keys);
            Assert.Equal("Expiry month must be in the future", errorDictionary["ExpiryMonth"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenExpiryMonthInThePastInCurrentYear()
        {
            // Arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year, ExpiryMonth = DateTime.Now.Month - 1 }; 

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("ExpiryYear", errorDictionary.Keys);
            Assert.Equal("Expiry year must be in the future", errorDictionary["ExpiryYear"]);
            Assert.Contains("ExpiryMonth", errorDictionary.Keys);
            Assert.Equal("Expiry month must be in the future", errorDictionary["ExpiryMonth"]);
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenExpiryDateInTheFuture()
        {
            // Arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year, ExpiryMonth = DateTime.Now.Month + 1 }; 

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
