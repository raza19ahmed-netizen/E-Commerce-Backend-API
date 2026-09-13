using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("order/{orderId}")]
    public async Task<IActionResult> CreatePayment(int orderId , CreatePaymentDto dto)
    {
        // JWT Token से logged-in User की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var payment = await _paymentService
            .CreatePaymentAsync(userId, orderId, dto);

        return Ok(new ApiResponse<PaymentDto>(
            true,
            "Payment created successfully.",
            payment));
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetPayment(int orderId)
    {
        // JWT Token से logged-in User की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var payment = await _paymentService
            .GetPaymentByOrderIdAsync(userId, orderId);

        if (payment == null)
        {
            return NotFound(new ApiResponse<object>(
                false,
                "Payment not found.",
                null));
        }

        return Ok(new ApiResponse<PaymentDto>(
            true,
            "Payment retrieved successfully.",
            payment));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{paymentId}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(
    int paymentId,
    UpdatePaymentStatusDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var payment = await _paymentService.UpdateStatusAsync(
            paymentId,
            userId,
            dto);

        return Ok(new ApiResponse<PaymentDto>(
            true,
            "Payment status updated successfully.",
            payment));
    }
}