using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IRefundRepository
{
    Task<Refund> AddAsync(Refund refund);

    Task<Refund?> GetByIdAsync(int id);

    Task<Refund?> GetByPaymentIdAsync(int paymentId);

    Task UpdateAsync(Refund refund);

    Task<IEnumerable<Refund>> GetAllAsync();
}