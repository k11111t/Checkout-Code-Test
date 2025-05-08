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
        public void Validate_ReturnsFalseWhenExpiryYearInThePast()
        {
            // arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year - 1, ExpiryMonth = DateTime.Now.Month };

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
        public void Validate_ReturnsFalseWhenExpiryMonthInThePastOfCurrentYear()
        {
            // arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year, ExpiryMonth = DateTime.Now.Month - 1 }; 

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
        public void Validate_ReturnsTrueWhenExpiryDateInTheFuture()
        {
            // arrange
            var request = new PostPaymentRequest { ExpiryYear = DateTime.Now.Year, ExpiryMonth = DateTime.Now.Month+1 }; 

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
