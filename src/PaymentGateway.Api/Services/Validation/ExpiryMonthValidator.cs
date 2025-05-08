using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

public class ExpiryMonthValidator : IValidator<PostPaymentRequest>
{
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary)
    {
        errorDictionary = new();
        if(request.ExpiryMonth < 1 || request.ExpiryMonth > 12)
        {
            errorDictionary["ExpiryMonth"] = "Expiry month must be between 1 and 12";
            return false;
        }
        return true;
    }
}