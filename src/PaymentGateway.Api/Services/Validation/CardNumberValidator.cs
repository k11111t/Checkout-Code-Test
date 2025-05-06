using System.Numerics;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

public class CardNumberValidator : IValidator<PostPaymentRequest> {
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary) {
        errorDictionary = new();
        if (request.CardNumber.Length < 14 || request.CardNumber.Length > 19 && !BigInteger.TryParse(request.CardNumber, out _))
        {
            errorDictionary["CardNumber"] = "Card number must be between 14 and 19 digits.";
            return false;
        }
        return true;
    }
}