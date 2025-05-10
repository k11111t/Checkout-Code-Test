using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Data;

namespace PaymentGateway.Api.Tests.Mappers
{
    public class PaymentRecordMapperTests
    {
        private readonly PaymentRecordMapper mapper = new();

        [Fact]
        public void CreatePaymentRecord_ReturnsNull_WhenRequestIsNull()
        {
            var result = mapper.CreatePaymentRecord(null);
            Assert.Null(result);
        }

        [Fact]
        public void CreatePaymentRecord_ReturnsMappedPaymentRecord_OnValidRequest()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = "USD",
                Amount = 9
            };

            var result = mapper.CreatePaymentRecord(request);

            Assert.NotNull(result);
            Assert.Equal(PaymentStatus.Authorized, result!.Status);
            Assert.Equal(5678, result.CardNumberLastFour);  // last four digits of the card
            Assert.Equal(request.ExpiryMonth, result.ExpiryMonth);
            Assert.Equal(request.ExpiryYear, result.ExpiryYear);
            Assert.Equal(request.Currency, result.Currency);
            Assert.Equal(request.Amount, result.Amount);
        }

        [Fact]
        public void CreatePaymentRecord_SetLastFourDigitsToZero_WhenCardNumberIsLessThanFourDigits()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "123",
                ExpiryMonth = 5,
                ExpiryYear = 2027,
                Currency = "GBP",
                Amount = 5
            };

            var result = mapper.CreatePaymentRecord(request);

            Assert.NotNull(result);
            Assert.Equal(0, result.CardNumberLastFour);
        }

        [Fact]
        public void CreatePaymentRecord_SetsLastFourDigitsToZero_WhenCardNumberIsNull()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = null,
                ExpiryMonth = 7,
                ExpiryYear = 2026,
                Currency = "EUR",
                Amount = 7
            };

            var result = mapper.CreatePaymentRecord(request);

            Assert.NotNull(result);
            Assert.Equal(0, result.CardNumberLastFour);  // card number is null, so it's zero
        }

        [Fact]
        public void CreatePaymentRecord_SetsLastFourDigitsToZero_WhenCardNumberContainsAlphabeticalCharacters()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234abcd5678",
                ExpiryMonth = 11,
                ExpiryYear = 2028,
                Currency = "CAD",
                Amount = 1
            };

            var result = mapper.CreatePaymentRecord(request);

            Assert.NotNull(result);
            Assert.Equal(0, result.CardNumberLastFour);  // non-numeric characters are ignored and last 4 digits are extracted correctly
        }

        [Fact]
        public void CreatePaymentRecord_SetsCurrencyToEmpty_WhenCurrencyIsNull()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 8,
                ExpiryYear = 2024,
                Currency = null,
                Amount = 2
            };

            var result = mapper.CreatePaymentRecord(request);

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Currency);  // currency is null in the request, so it should be an empty string in the result
        }

        [Fact]
        public void CreatePaymentRecord_SetsCurrencyToEmpty_WhenCurrencyEmpty()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 8,
                ExpiryYear = 2024,
                Currency = string.Empty,
                Amount = 2
            };

            var result = mapper.CreatePaymentRecord(request);

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Currency);  // currency is null in the request, so it should be an empty string in the result
        }
    }
}
