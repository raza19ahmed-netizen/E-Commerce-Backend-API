using E_commerce.API.DTOs;

namespace E_commerce.API.Interfaces;

public interface IRefundService
{
    Task<RefundDto> CreateRefundAsync(int userId, int paymentId);

    Task<RefundDto?> GetRefundByPaymentIdAsync(
        int userId,
        int paymentId);

    Task<IEnumerable<RefundDto>> GetAllAsync();

    Task<RefundDto> UpdateStatusAsync(
    int refundId,
    UpdateRefundStatusDto dto);

    
}