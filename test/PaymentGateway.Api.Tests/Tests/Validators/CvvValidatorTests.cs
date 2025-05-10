using PaymentGateway.Api.Models.Requests;
using Moq;
using PaymentGateway.Api.Interfaces;

namespace PaymentGateway.Api.Tests.Tests
{
    public class CvvValidatorTests
    {
        readonly CvvValidator validator;

        public CvvValidatorTests()
        {
            validator = new CvvValidator();
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCvvGreaterThanFourDigits()
        {
            // Arrange
            var request = new PostPaymentRequest { Cvv = 12345 };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("Cvv", errorDictionary.Keys);
            Assert.Equal("Cvv must be between 3 to 4 digits", errorDictionary["Cvv"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenCvvNegative()
        {
            // Arrange
            var request = new PostPaymentRequest { Cvv = -1 };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.False(result);
            Assert.Contains("Cvv", errorDictionary.Keys);
            Assert.Equal("Cvv must be between 3 to 4 digits", errorDictionary["Cvv"]);
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenCvvValid()
        {
            // Arrange
            var request = new PostPaymentRequest { Cvv = 1234 };

            // Act
            var result = validator.Validate(request, out var errorDictionary);

            // Assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
