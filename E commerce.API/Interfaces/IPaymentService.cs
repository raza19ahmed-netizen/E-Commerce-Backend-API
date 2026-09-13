using E_commerce.API.DTOs;

namespace E_commerce.API.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(int userId, int orderId, CreatePaymentDto dto);

    Task<PaymentDto?> GetPaymentByOrderIdAsync(int userId, int orderId);

    Task<PaymentDto> UpdateStatusAsync(
    int paymentId,
    int userId,
    UpdatePaymentStatusDto dto);


}