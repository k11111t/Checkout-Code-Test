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
        public void Validate_ReturnsFalseWhenExpiryYearInThePast()
        {
            // arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year - 1 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("ExpiryYear", errorDictionary.Keys);
            Assert.Equal("Expiry year must be in the future", errorDictionary["ExpiryYear"]);
        }

        [Fact]
        public void Validate_ReturnsTrueWhenExpiryYearInTheFuture()
        {
            // arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year + 1 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
