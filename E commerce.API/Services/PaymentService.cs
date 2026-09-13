using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<PaymentDto> CreatePaymentAsync(
        int userId,
        int orderId,CreatePaymentDto dto)
    {
        // Step 1: Order प्राप्त करो
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        // Step 2: Check करो कि Order उसी logged-in User का है
        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to access this order.");
        }

        // Step 3: Check करो कि इस Order की Payment पहले से मौजूद है
        var existingPayment = await _paymentRepository
            .GetByOrderIdAsync(orderId);

        if (existingPayment != null)
        {
            throw new InvalidOperationException(
                "Payment already exists for this order.");
        }

        // Step 4: नया Payment बनाओ
        var paymentMethod = dto.PaymentMethod.Trim();

        if (paymentMethod != "COD" && paymentMethod != "Online")
        {
            throw new InvalidOperationException(
                "Invalid payment method. Allowed methods are COD and Online.");
        }

        var payment = new Payment
        {
            OrderId = orderId,
            Order = order,
            Amount = order.TotalAmount,
            Status = "Pending",
            PaymentMethod = paymentMethod
        };

        if (paymentMethod == "COD")
        {
            order.Status = "Confirmed";
        }

        // Step 5: Database में Payment save करो
        var savedPayment = await _paymentRepository
            .AddAsync(payment);

        // Step 6: DTO response बनाओ
        return new PaymentDto
        {
            Id = savedPayment.Id,
            OrderId = savedPayment.OrderId,
            Amount = savedPayment.Amount,
            Status = savedPayment.Status,
            PaymentMethod = savedPayment.PaymentMethod,
            TransactionId = savedPayment.TransactionId,
            CreatedAt = savedPayment.CreatedAt,
            PaidAt = savedPayment.PaidAt

        };
    }

    public async Task<PaymentDto?> GetPaymentByOrderIdAsync(
        int userId,
        int orderId)
    {
        // Order प्राप्त करो
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        // Ownership check
        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to access this order.");
        }

        // Payment प्राप्त करो
        var payment = await _paymentRepository
            .GetByOrderIdAsync(orderId);

        if (payment == null)
        {
            return null;
        }

        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status,
            TransactionId = payment.TransactionId,
            CreatedAt = payment.CreatedAt,
            PaidAt = payment.PaidAt,
            PaymentMethod = payment.PaymentMethod
        };
    }
    public async Task<PaymentDto> UpdateStatusAsync(
    int paymentId,
    int userId,
    UpdatePaymentStatusDto dto)
    {
        // Payment प्राप्त करो
        var payment = await _paymentRepository
            .GetByIdAsync(paymentId);

        if (payment == null)
        {
            throw new KeyNotFoundException(
                "Payment not found.");
        }

       
        // सिर्फ allowed status accept करो
        var status = dto.Status.Trim();

        if (status != "Success" &&
            status != "Failed")
        {
            throw new InvalidOperationException(
                "Invalid payment status.");
        }

        if (payment.Status == "Success")
        {
            throw new InvalidOperationException(
                "Successful payment cannot be updated again.");
        }

        if (payment.Status == "Refunded")
        {
            throw new InvalidOperationException(
                "Refunded payment cannot be updated.");
        }

        // Payment update
        payment.Status = status;
        payment.TransactionId = dto.TransactionId;

        if (status == "Success")
        {
            payment.PaidAt = DateTime.UtcNow;


            if (payment.Order.Status == "Pending")// sirf pending order ko confirm karo

            {
                // Payment successful → Order Confirmed
                payment.Order.Status = "Confirmed";
            }

        }
        else
        {
            payment.PaidAt = null;

            if (payment.Order.Status == "Confirmed")// sirf confirmed order ko pending karo
            {

                // Payment failed → Order वापस Pending
                payment.Order.Status = "Pending";
            }
        }

        // Payment + Order दोनों save होंगे
        await _paymentRepository.UpdateAsync(payment);

        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status,
            PaymentMethod = payment.PaymentMethod,
            TransactionId = payment.TransactionId,
            CreatedAt = payment.CreatedAt,
            PaidAt = payment.PaidAt
        };
    }
}