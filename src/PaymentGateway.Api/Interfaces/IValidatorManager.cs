namespace PaymentGateway.Api.Interfaces;
public interface IValidatorManager<T>
{
    bool Validate(T value, out Dictionary<string, string> errorDictionary);
}