using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.PaymentGateway;
public class PaymentManager : IPaymentManager
{

    private readonly IValidatorManager<PostPaymentRequest> _postRequestValidationService;
    private readonly IRepository<PaymentRecord> _paymentsRepository;
    private readonly IBankPaymentManager _bankPaymentManager;
    private readonly ILogger<PaymentManager> _logger;
    private readonly IBankPaymentMapper _bankMapper;
    private readonly IPaymentRecordMapper _recordMapper;
    private readonly IHttpPaymentMapper _httpMapper;

    public PaymentManager(
        IRepository<PaymentRecord> paymentsRepository, 
        IValidatorManager<PostPaymentRequest> postRequestValidationService,
        IBankPaymentManager bankPaymentManager,
        IBankPaymentMapper bankMapper,
        IPaymentRecordMapper recordMapper,
        IHttpPaymentMapper httpMapper,
        ILogger<PaymentManager> logger)
    {
        _paymentsRepository = paymentsRepository;
        _postRequestValidationService = postRequestValidationService;
        _bankPaymentManager = bankPaymentManager;
        _httpMapper = httpMapper;
        _bankMapper = bankMapper;
        _recordMapper = recordMapper;
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
        BankPaymentRequest? bankRequest = _bankMapper.CreateBankPaymentRequest(request);
        if(bankRequest == null)
        {
            _logger.LogError("Bank request is invalid");
            result.Status = PaymentStatus.Rejected;
            return result;
        }

        BankPaymentResponse? bankResponse = await _bankPaymentManager.ProcessBankPaymentAsync(bankRequest);

        if(bankResponse == null)
        {
            _logger.LogError("Bank service failed to process payment");
            result.Status = PaymentStatus.Rejected;
            return result;
        }

        // create payment record
        _logger.LogInformation("Creating payment record");
        PaymentRecord? payment = _recordMapper.CreatePaymentRecord(request);
        if(payment == null)
        {
            _logger.LogError("Failed to create payment record");
            result.Status = PaymentStatus.Rejected;
            return result;
        }

        // transaction failed
        if(bankResponse.Authorized == false)
            payment.Status = PaymentStatus.Declined;
        
        // record payment in DB
        _paymentsRepository.Add(payment);
        _logger.LogInformation("Payment successfully recorded");

        //create response to client
        var postPaymentResponse = _httpMapper.CreatePostResponse(payment);

        if(postPaymentResponse == null)
        {
            _logger.LogError("Failed to create post response");
            result.Status = PaymentStatus.Rejected;
            return result;
        }

        return postPaymentResponse;
    }

    public async Task<GetPaymentResponse?> GetPaymentAsync(Guid id)
    {
        PaymentRecord? payment = _paymentsRepository.Get(id);
        if(payment == null) return null;
        GetPaymentResponse? response = _httpMapper.CreateGetResponse(payment);
        return response;
    }
}