using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> AddAsync(Payment payment); // payment add in data base

    Task<Payment?> GetByIdAsync(int id);// payment ki id se payment find karna

    Task<Payment?> GetByOrderIdAsync(int orderId);// order id se payment find karo.

    Task UpdateAsync(Payment payment);// existing payment update karna
}