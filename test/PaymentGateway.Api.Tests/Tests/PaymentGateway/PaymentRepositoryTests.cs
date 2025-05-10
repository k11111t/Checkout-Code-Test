using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Services.PaymentGateway;

namespace PaymentGateway.Api.Tests.Tests
{
    public class PaymentsRepositoryTests
    {
        readonly PaymentsRepository repository;

        public PaymentsRepositoryTests()
        {
            repository = new PaymentsRepository();
        }

        [Fact]
        public void Add_AddsPaymentToList()
        {
            // Arrange
            var payment = new PaymentRecord(){
                Id = Guid.NewGuid() 
            };

            // Act
            repository.Add(payment);

            // Assert
            var result = repository.Get(payment.Id);
            Assert.NotNull(result);
            Assert.Equal(payment.Id, result.Id);
        }

        [Fact]
        public void Get_ReturnsNull_WhenPaymentMissing()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = repository.Get(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Get_ReturnsPayment_WhenPaymentExists()
        {
            // Arrange
            var expectedPayment = new PaymentRecord { Id = Guid.NewGuid() };
            repository.Add(expectedPayment);

            // Act
            var result = repository.Get(expectedPayment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPayment.Id, result.Id);
        }

        [Fact]
        public void Add_ReturnsCorrectPayment_WehnAddingMultiplePayments()
        {
            // Arrange
            var expectedPayment1 = new PaymentRecord { Id = Guid.NewGuid() };
            var expectedPayment2 = new PaymentRecord { Id = Guid.NewGuid() };

            // Act
            repository.Add(expectedPayment1);
            repository.Add(expectedPayment2);

            // Assert
            var result1 = repository.Get(expectedPayment1.Id);
            var result2 = repository.Get(expectedPayment2.Id);

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(expectedPayment1.Id, result1.Id);
            Assert.Equal(expectedPayment2.Id, result2.Id);
        }
    }
}
