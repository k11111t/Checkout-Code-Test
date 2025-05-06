using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

public class ExpiryYearValidator : IValidator<PostPaymentRequest> {
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary) {
        errorDictionary = new();
        if(request.ExpiryYear < DateTime.Now.Year) {
            errorDictionary["ExpiryYear"] = "Expiry year must be in the future";
            return false;
        }
        return true;
    }
}