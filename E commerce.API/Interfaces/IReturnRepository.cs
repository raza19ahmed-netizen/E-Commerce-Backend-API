using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IReturnRepository
{
    Task<ReturnRequest> AddAsync(ReturnRequest returnRequest);

    Task<ReturnRequest?> GetByIdAsync(int id);

    Task<ReturnRequest?> GetByOrderIdAsync(int orderId);

    Task<IEnumerable<ReturnRequest>> GetByUserIdAsync(int userId);

    Task UpdateAsync(ReturnRequest returnRequest);

    Task<IEnumerable<ReturnRequest>> GetAllAsync();
}