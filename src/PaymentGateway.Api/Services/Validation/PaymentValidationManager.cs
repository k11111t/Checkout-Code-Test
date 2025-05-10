using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Validation;
public class PaymentValidationManager : IValidatorManager<PostPaymentRequest>
{
   private readonly IEnumerable<IValidator<PostPaymentRequest>> _validators;
    public PaymentValidationManager(IEnumerable<IValidator<PostPaymentRequest>> validators)
    {
        _validators = validators;
    }

    public bool Validate(PostPaymentRequest request, out Dictionary<string, string> errorMessages)
    {
        errorMessages = new();

        if(request == null)
            return false;

        foreach (var validator in _validators) {
            if(!validator.Validate(request, out var errorDictionary))
            {
                foreach(var error in errorDictionary)
                {
                    errorMessages[error.Key] = error.Value;
                }
            }
        }
        return errorMessages.Count == 0;
    }

}