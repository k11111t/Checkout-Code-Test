using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.PaymentGateway;
public class PaymentManager : IPaymentManager {

    private readonly IValidatorManager<PostPaymentRequest> _postRequestValidationService;
    private readonly IRepository<PaymentRecord> _paymentsRepository;
    private readonly IBankPaymentManager _bankPaymentManager;

    public PaymentManager(
        IRepository<PaymentRecord> paymentsRepository, 
        IValidatorManager<PostPaymentRequest> postRequestValidationService,
        IBankPaymentManager bankPaymentManager)
    {
        _paymentsRepository = paymentsRepository;
        _postRequestValidationService = postRequestValidationService;
        _bankPaymentManager = bankPaymentManager;
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request) {
        bool validationPassed = _postRequestValidationService.Validate(request, out var errorDetails);

        PaymentRecord payment = new() {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = int.Parse(request.CardNumber.Substring(request.CardNumber.Length-5, 4)),
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            Amount = request.Amount
        };

        PostPaymentResponse response = MapToPostResponse(payment);

        if (!validationPassed)
        {
            payment.Status = PaymentStatus.Rejected;
            response.Status = PaymentStatus.Rejected;
            _paymentsRepository.Add(payment);
            return response;
        }

        // send request to bank
        BankPaymentRequest bankRequest = new BankPaymentRequest() {
            CardNumber = request.CardNumber,
            ExpiryDate =  $"{request.ExpiryMonth}/{request.ExpiryYear}",
            Currency = request.Currency,
            Amount = request.Amount,
            Cvv = request.Cvv.ToString()
        };

        BankPaymentResponse bankResponse = await _bankPaymentManager.ProcessBankPaymentAsync(bankRequest);

        if(bankResponse.Authorized == false)
        {
            payment.Status = PaymentStatus.Declined;
            response.Status = PaymentStatus.Declined;
            _paymentsRepository.Add(payment);
            return response;
        }

        // transaction was successful
        _paymentsRepository.Add(payment);
        return response;
    }

    public async Task<GetPaymentResponse> GetPaymentAsync(Guid id){
        PaymentRecord payment = await Task.Run(() => _paymentsRepository.Get(id));
        if(payment == null) return null;
        GetPaymentResponse response = MapToGetResponse(payment);
        return response;
    }

    private static PostPaymentResponse MapToPostResponse(PaymentRecord payment)
    {
        return new PostPaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }

    private static GetPaymentResponse MapToGetResponse(PaymentRecord payment)
    {
        return new GetPaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }

}