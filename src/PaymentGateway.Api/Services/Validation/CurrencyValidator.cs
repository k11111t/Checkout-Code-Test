using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

public class CurrencyValidator : IValidator<PostPaymentRequest>
{
    private readonly IPaymentGatewayConfiguration _config;
    public CurrencyValidator(IPaymentGatewayConfiguration config)
    {
        _config = config;
    }
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary)
    {
        errorDictionary = new();

        if(request == null)
        {
            errorDictionary["Currency"] = $"Currency {request.Currency} is not supported.";
            return false;
        }

        if(!_config.SupportedCurrencies.Contains(request.Currency))
        {
            errorDictionary["Currency"] = $"Currency {request.Currency} is not supported.";
            return false;
        }
        return true;
    }
}