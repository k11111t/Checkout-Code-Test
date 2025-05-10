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
        public void Validate_ReturnsFalse_WhenInputNull()
        {
            // arrange
            PostPaymentRequest request = null;

            // act
            var result = validator.Validate(request, out var errorDictionary);

            // assert
            Assert.False(result);
            Assert.Contains("Amount", errorDictionary.Keys);
            Assert.Equal("Amount must be greater than zero.", errorDictionary["Amount"]);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenAmountZero()
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
        public void Validate_ReturnsFalse_WhenAmountIsNegative()
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
        public void Validate_ReturnsTrue_WhenAmountIsPositive()
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
