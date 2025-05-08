using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.Tests
{
    public class CardNumberValidatorTests
    {
        readonly CardNumberValidator validator;

        public CardNumberValidatorTests()
        {
            validator = new CardNumberValidator();
        }

        [Fact]
        public void Validate_ReturnsFalseWhenCardNumberIsNull()
        {
            // arrange
            var request = new PostPaymentRequest { CardNumber = null };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsFalseWhenCardNumberTooShort()
        {
            // arrange
            var request = new PostPaymentRequest { CardNumber = "123456789" };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsFalseWhenCardNumberTooLong()
        {
            // arrange
            var request = new PostPaymentRequest { CardNumber = "12345678901234567890" };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsFalseWhenCardNumberContainsNonNumericals()
        {
            // arrange
            var request = new PostPaymentRequest { CardNumber = "12345A7890123" };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("CardNumber", errorDictionary.Keys);
            Assert.Equal("Card number must be between 14 and 19 digits.", errorDictionary["CardNumber"]);
        }

        [Fact]
        public void Validate_ReturnsTrueWhenCardNumberIsValid()
        {
            // arrange
            var request = new PostPaymentRequest { CardNumber = "1234567891111111" };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
