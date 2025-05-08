using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;

public class CardNumberValidator : IValidator<PostPaymentRequest>
{
    public bool Validate(PostPaymentRequest request, out Dictionary<string,string> errorDictionary)
    {
        errorDictionary = new();
        if (request.CardNumber == null || request.CardNumber.Length < 14 || request.CardNumber.Length > 19 || !CheckAllDigits(request.CardNumber))
        {
            errorDictionary["CardNumber"] = "Card number must be between 14 and 19 digits.";
            return false;
        }
        return true;
    }

    private bool CheckAllDigits(string cardNumber)
    {
        return cardNumber.All(c => char.IsDigit(c));
    }
}