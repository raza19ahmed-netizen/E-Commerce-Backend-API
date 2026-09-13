using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class ReturnService : IReturnService
{
    private readonly IReturnRepository _returnRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IRefundService _refundService;

    public ReturnService(
        IReturnRepository returnRepository,
        IOrderRepository orderRepository, IRefundService refundService)
    {
        _returnRepository = returnRepository;
        _orderRepository = orderRepository;
        _refundService = refundService;
    }

    public async Task<ReturnRequestDto> CreateReturnAsync(
        int userId,
        int orderId,
        CreateReturnRequestDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        if (order.UserId != userId)
            throw new UnauthorizedAccessException(
                "You are not allowed to return this order.");

        if (order.Status != "Delivered")
            throw new InvalidOperationException(
                "Only delivered orders can be returned.");

        if (string.IsNullOrWhiteSpace(dto.Reason))
            throw new InvalidOperationException(
                "Return reason is required.");

        var existingReturn =
            await _returnRepository.GetByOrderIdAsync(orderId);

        if (existingReturn != null)
            throw new InvalidOperationException(
                "Return request already exists for this order.");

        var returnRequest = new ReturnRequest
        {
            OrderId = orderId,
            UserId = userId,
            Reason = dto.Reason.Trim(),
            Status = "Pending"
        };

        var savedReturn =
            await _returnRepository.AddAsync(returnRequest);

        return new ReturnRequestDto
        {
            Id = savedReturn.Id,
            OrderId = savedReturn.OrderId,
            Reason = savedReturn.Reason,
            Status = savedReturn.Status,
            CreatedAt = savedReturn.CreatedAt,
            ProcessedAt = savedReturn.ProcessedAt
        };
    }

    public async Task<IEnumerable<ReturnRequestDto>> GetMyReturnsAsync(
        int userId)
    {
        var returns =
            await _returnRepository.GetByUserIdAsync(userId);

        return returns.Select(r => new ReturnRequestDto
        {
            Id = r.Id,
            OrderId = r.OrderId,
            Reason = r.Reason,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            ProcessedAt = r.ProcessedAt
        });
    }

    public async Task<ReturnRequestDto?> GetReturnByOrderIdAsync(
        int userId,
        int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        if (order.UserId != userId)
            throw new UnauthorizedAccessException(
                "You are not allowed to view this return.");

        var returnRequest =
            await _returnRepository.GetByOrderIdAsync(orderId);

        if (returnRequest == null)
            return null;

        return new ReturnRequestDto
        {
            Id = returnRequest.Id,
            OrderId = returnRequest.OrderId,
            Reason = returnRequest.Reason,
            Status = returnRequest.Status,
            CreatedAt = returnRequest.CreatedAt,
            ProcessedAt = returnRequest.ProcessedAt
        };
    }

    public async Task<ReturnRequestDto> UpdateStatusAsync(
        int returnId,
        UpdateReturnStatusDto dto)
    {
        var returnRequest =
            await _returnRepository.GetByIdAsync(returnId);

        if (returnRequest == null)
            throw new KeyNotFoundException(
                "Return request not found.");

        var status = dto.Status.Trim();

        if (status != "Approved" && status != "Rejected")
            throw new InvalidOperationException(
                "Invalid return status.");

        if (returnRequest.Status != "Pending")
            throw new InvalidOperationException(
                "Return request has already been processed.");

        returnRequest.Status = status;
        returnRequest.ProcessedAt = DateTime.UtcNow;

        if (status == "Approved")
        {
            if (returnRequest.Order != null)
            {
                returnRequest.Order.Status = "ReturnApproved";

                if (returnRequest.Order.Payment != null &&
                    returnRequest.Order.Payment.Status == "Success")
                {
                    await _refundService.CreateRefundAsync(
                        returnRequest.UserId,
                        returnRequest.Order.Payment.Id);
                }
            }
        }

        await _returnRepository.UpdateAsync(returnRequest);

        return new ReturnRequestDto
        {
            Id = returnRequest.Id,
            OrderId = returnRequest.OrderId,
            Reason = returnRequest.Reason,
            Status = returnRequest.Status,
            CreatedAt = returnRequest.CreatedAt,
            ProcessedAt = returnRequest.ProcessedAt
        };
    }

    public async Task<IEnumerable<ReturnRequestDto>> GetAllAsync()
    {
        var returns = await _returnRepository.GetAllAsync();

        return returns.Select(r => new ReturnRequestDto
        {
            Id = r.Id,
            OrderId = r.OrderId,
            Reason = r.Reason,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            ProcessedAt = r.ProcessedAt
        }).ToList();
    }
}