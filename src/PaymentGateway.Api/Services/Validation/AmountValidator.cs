using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

public class AmountValidator : IValidator<PostPaymentRequest>
{
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary)
    {
        errorDictionary = new();
        if(request.Amount <= 0)
        {
            errorDictionary["Amount"] = "Amount must be greater than zero.";
            return false;
        }
        return true;
    }
}