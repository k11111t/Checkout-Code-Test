using System.Collections.Concurrent;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Data;

namespace PaymentGateway.Api.Services.PaymentGateway;

public class PaymentsRepository : IRepository<PaymentRecord>
{
    public ConcurrentBag<PaymentRecord> Payments = new();
    
    public void Add(PaymentRecord payment)
    {
        Payments.Add(payment);
    }

    public PaymentRecord? Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}