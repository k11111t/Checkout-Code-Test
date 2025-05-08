namespace PaymentGateway.Api.Interfaces;
public interface IValidator<T>
{
    bool Validate(T value, out Dictionary<string, string> errorDictionary);
}