using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Validation;
public class ExpiryYearValidator : IValidator<PostPaymentRequest>
{
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary)
    {
        errorDictionary = new();

        if(request == null)
        {
            errorDictionary["ExpiryYear"] = "Expiry year must be in the future";
            return false;
        }
        
        if(request.ExpiryYear < DateTime.Now.Year)
        {
            errorDictionary["ExpiryYear"] = "Expiry year must be in the future";
            return false;
        }
        return true;
    }
}