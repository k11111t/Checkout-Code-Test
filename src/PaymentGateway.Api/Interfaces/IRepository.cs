namespace PaymentGateway.Api.Interfaces
{
    public interface IRepository<T> where T : class
    {    
        void Add(T entity);
        T Get(Guid id);
    }
}