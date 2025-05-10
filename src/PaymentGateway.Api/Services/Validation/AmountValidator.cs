using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Validation;
public class AmountValidator : IValidator<PostPaymentRequest>
{
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary)
    {
        errorDictionary = new();
        if(request == null)
        {
            errorDictionary["Amount"] = "Amount must be greater than zero.";
            return false;
        }
            

        if(request.Amount <= 0)
        {
            errorDictionary["Amount"] = "Amount must be greater than zero.";
            return false;
        }
        return true;
    }
}