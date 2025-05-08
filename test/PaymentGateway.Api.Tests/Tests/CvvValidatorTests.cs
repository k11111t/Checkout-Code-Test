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
        public void Validate_ReturnsFalseWhenCvvLessThanThreeDigits()
        {
            // arrange
            var request = new PostPaymentRequest { Cvv = 12 }; 

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("Cvv", errorDictionary.Keys);
            Assert.Equal("Cvv must be between 3 to 4 digits", errorDictionary["Cvv"]);
        }

        [Fact]
        public void Validate_ReturnsFalseWhenCvvGreaterThanFourDigits()
        {
            // arrange
            var request = new PostPaymentRequest { Cvv = 12345 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("Cvv", errorDictionary.Keys);
            Assert.Equal("Cvv must be between 3 to 4 digits", errorDictionary["Cvv"]);
        }

        [Fact]
        public void Validate_ReturnsTrueWhenCvvValid()
        {
            // arrange
            var request = new PostPaymentRequest { Cvv = 1234 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
