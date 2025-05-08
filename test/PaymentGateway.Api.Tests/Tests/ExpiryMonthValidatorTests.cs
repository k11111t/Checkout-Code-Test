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
        public void Validate_ReturnsFalseWhenExpiryMonthLessThanOne()
        {
            // arrange
            var request = new PostPaymentRequest { ExpiryMonth = 0 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("ExpiryMonth", errorDictionary.Keys);
            Assert.Equal("Expiry month must be between 1 and 12", errorDictionary["ExpiryMonth"]);
        }

        [Fact]
        public void Validate_ReturnsFalseWhenExpiryMonthIsGreaterThanTwelve()
        {
            // arrange
            var request = new PostPaymentRequest { ExpiryMonth = 13 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("ExpiryMonth", errorDictionary.Keys);
            Assert.Equal("Expiry month must be between 1 and 12", errorDictionary["ExpiryMonth"]);
        }

        [Fact]
        public void Validate_ReturnsTrueWhenExpiryMonthValid()
        {
            // arrange
            var request = new PostPaymentRequest { ExpiryMonth = 5 };  // Valid expiry month (between 1 and 12)

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
