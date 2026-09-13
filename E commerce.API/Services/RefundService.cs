using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class RefundService : IRefundService
{
    private readonly IRefundRepository _refundRepository;
    private readonly IPaymentRepository _paymentRepository;

    public RefundService(
        IRefundRepository refundRepository,
        IPaymentRepository paymentRepository)
    {
        _refundRepository = refundRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<RefundDto> CreateRefundAsync(
        int userId,
        int paymentId)
    {
        // Step 1: Payment find karo
        var payment = await _paymentRepository.GetByIdAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException("Payment not found.");

        // Step 2: Payment owner check karo
        if (payment.Order.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to refund this payment.");
        }

        // Step 3: Payment Success hona chahiye
        if (payment.Status != "Success")
        {
            throw new InvalidOperationException(
                "Only successful payments can be refunded.");
        }

        if (payment.Amount <= 0)
        {
            throw new InvalidOperationException(
                "Invalid payment amount for refund.");
        }

        // Step 4: Existing refund check
        var existingRefund =
            await _refundRepository.GetByPaymentIdAsync(paymentId);

        if (existingRefund != null)
        {
            throw new InvalidOperationException(
                "Refund already exists for this payment.");
        }

        // Step 5: Refund create karo
        var refund = new Refund
        {
            PaymentId = paymentId,
            Amount = payment.Amount,
            Status = "Pending"
        };

        var savedRefund =
            await _refundRepository.AddAsync(refund);

        // Step 6: DTO return karo
        return new RefundDto
        {
            Id = savedRefund.Id,
            PaymentId = savedRefund.PaymentId,
            Amount = savedRefund.Amount,
            Status = savedRefund.Status,
            RefundTransactionId =
                savedRefund.RefundTransactionId,
            CreatedAt = savedRefund.CreatedAt,
            RefundedAt = savedRefund.RefundedAt
        };
    }

    public async Task<RefundDto?> GetRefundByPaymentIdAsync(
        int userId,
        int paymentId)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException("Payment not found.");

        if (payment.Order.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to view this refund.");
        }

        var refund =
            await _refundRepository.GetByPaymentIdAsync(paymentId);

        if (refund == null)
            return null;

        return new RefundDto
        {
            Id = refund.Id,
            PaymentId = refund.PaymentId,
            Amount = refund.Amount,
            Status = refund.Status,
            RefundTransactionId =
                refund.RefundTransactionId,
            CreatedAt = refund.CreatedAt,
            RefundedAt = refund.RefundedAt
        };
    }

    public async Task<IEnumerable<RefundDto>> GetAllAsync()
    {
        var refunds = await _refundRepository.GetAllAsync();

        return refunds.Select(refund => new RefundDto
        {
            Id = refund.Id,
            PaymentId = refund.PaymentId,
            Amount = refund.Amount,
            Status = refund.Status,
            RefundTransactionId = refund.RefundTransactionId,
            CreatedAt = refund.CreatedAt,
            RefundedAt = refund.RefundedAt
        }).ToList();
    }

    public async Task<RefundDto> UpdateStatusAsync(
    int refundId,
    UpdateRefundStatusDto dto)
    {
        var refund = await _refundRepository.GetByIdAsync(refundId);

        if (refund == null)
            throw new KeyNotFoundException("Refund not found.");

        var status = dto.Status.Trim();

        if (status != "Success" && status != "Failed")
        {
            throw new InvalidOperationException(
                "Invalid refund status.");
        }

        if (refund.Status == "Success")
        {
            throw new InvalidOperationException(
                "Successful refund cannot be updated again.");
        }

        refund.Status = status;
        refund.RefundTransactionId = dto.RefundTransactionId;

        if (status == "Success")
        {
            refund.RefundedAt = DateTime.UtcNow;

            if (refund.Payment!= null)
            {
                refund.Payment.Status = "Refunded";
            }
        }
        else
        {
            refund.RefundedAt = null;
        }

        await _refundRepository.UpdateAsync(refund);

        return new RefundDto
        {
            Id = refund.Id,
            PaymentId = refund.PaymentId,
            Amount = refund.Amount,
            Status = refund.Status,
            RefundTransactionId = refund.RefundTransactionId,
            CreatedAt = refund.CreatedAt,
            RefundedAt = refund.RefundedAt
        };
    }
}