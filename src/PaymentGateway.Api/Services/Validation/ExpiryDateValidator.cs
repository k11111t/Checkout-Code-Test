using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Validation;
public class ExpiryDateValidator : IValidator<PostPaymentRequest>
{
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary)
    {
        errorDictionary = new();

        if(request == null)
        {
            errorDictionary["ExpiryYear"] = "Expiry year must be in the future";
            errorDictionary["ExpiryMonth"] = "Expiry month must be in the future";
            return false;
        }
        
        if(request.ExpiryYear < DateTime.Now.Year || (request.ExpiryYear == DateTime.Now.Year && request.ExpiryMonth < DateTime.Now.Month))
        {
            errorDictionary["ExpiryYear"] = "Expiry year must be in the future";
            errorDictionary["ExpiryMonth"] = "Expiry month must be in the future";
            return false;
        }
        return true;
    }
}