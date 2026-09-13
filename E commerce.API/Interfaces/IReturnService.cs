using E_commerce.API.DTOs;

namespace E_commerce.API.Interfaces;

public interface IReturnService
{
    Task<ReturnRequestDto> CreateReturnAsync(
        int userId,
        int orderId,
        CreateReturnRequestDto dto);

    Task<IEnumerable<ReturnRequestDto>> GetMyReturnsAsync(
        int userId);

    Task<ReturnRequestDto?> GetReturnByOrderIdAsync(
        int userId,
        int orderId);

    Task<ReturnRequestDto> UpdateStatusAsync(
        int returnId,
        UpdateReturnStatusDto dto);
    Task<IEnumerable<ReturnRequestDto>> GetAllAsync();
}