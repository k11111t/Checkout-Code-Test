using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Validation;
public class CvvValidator : IValidator<PostPaymentRequest>
{
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary)
    {
        errorDictionary = new();
        if(request == null)
        {
            errorDictionary["Cvv"] = "Cvv must be between 3 to 4 digits";
            return false;
        }

        if(request.Cvv < 0 || request.Cvv > 9999)
        {
            errorDictionary["Cvv"] = "Cvv must be between 3 to 4 digits";
            return false;
        }
        return true;
    }
}