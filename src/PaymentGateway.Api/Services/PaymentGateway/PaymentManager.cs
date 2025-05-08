using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Toolbox;

namespace PaymentGateway.Api.Services.PaymentGateway;
public class PaymentManager : IPaymentManager
{

    private readonly IValidatorManager<PostPaymentRequest> _postRequestValidationService;
    private readonly IRepository<PaymentRecord> _paymentsRepository;
    private readonly IBankPaymentManager _bankPaymentManager;
    private readonly ILogger<PaymentManager> _logger;

    public PaymentManager(
        IRepository<PaymentRecord> paymentsRepository, 
        IValidatorManager<PostPaymentRequest> postRequestValidationService,
        IBankPaymentManager bankPaymentManager,
        ILogger<PaymentManager> logger)
    {
        _paymentsRepository = paymentsRepository;
        _postRequestValidationService = postRequestValidationService;
        _bankPaymentManager = bankPaymentManager;
        _logger = logger;
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request)
    {
        bool validationPassed = _postRequestValidationService.Validate(request, out var errorDetails);
        PostPaymentResponse result = new();

        if (!validationPassed)
        {
            _logger.LogError("Failed to validate payment");
            result.Status = PaymentStatus.Rejected;
            return result;
        }

        // send request to bank
        BankPaymentRequest bankRequest = PaymentMapper.CreateBankPaymentRequest(request);
        BankPaymentResponse? bankResponse = await _bankPaymentManager.ProcessBankPaymentAsync(bankRequest);

        if(bankResponse == null)
        {
            _logger.LogError("Bank service failed to process payment");
            result.Status = PaymentStatus.Rejected;
            return result;
        }

        // create payment record
        _logger.LogInformation("Creating payment record");
        PaymentRecord payment = PaymentMapper.CreatePaymentRecord(request);

        // transaction failed
        if(bankResponse.Authorized == false)
            payment.Status = PaymentStatus.Declined;
        
        // record payment in DB
        _paymentsRepository.Add(payment);
        _logger.LogInformation("Payment successfully recorded");

        //create response to client
        result = PaymentMapper.CreatePostResponse(payment);

        return result;
    }

    public async Task<GetPaymentResponse?> GetPaymentAsync(Guid id)
    {
        PaymentRecord? payment = _paymentsRepository.Get(id);
        if(payment == null) return null;
        GetPaymentResponse response = PaymentMapper.CreateGetResponse(payment);
        return response;
    }
}