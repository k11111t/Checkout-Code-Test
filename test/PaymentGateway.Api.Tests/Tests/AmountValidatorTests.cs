using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.Tests
{
    public class AmountValidatorTests
    {
        readonly AmountValidator validator;

        public AmountValidatorTests()
        {
            validator = new AmountValidator();
        }

        [Fact]
        public void Validate_ReturnsFalseWhenAmountZero()
        {
            // arrange
            var request = new PostPaymentRequest { Amount = 0 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("Amount", errorDictionary.Keys);
            Assert.Equal("Amount must be greater than zero.", errorDictionary["Amount"]);
        }

        [Fact]
        public void Validate_ReturnsFalseWhenAmountIsNegative()
        {
            // arrange
            var request = new PostPaymentRequest { Amount = -10 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("Amount", errorDictionary.Keys);
            Assert.Equal("Amount must be greater than zero.", errorDictionary["Amount"]);
        }

        [Fact]
        public void Validate_ReturnsTrueWhenAmountIsPositive()
        {
            // arrange
            var request = new PostPaymentRequest { Amount = 10 };

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.True(result);
            Assert.Empty(errorDictionary);
        }
    }
}
