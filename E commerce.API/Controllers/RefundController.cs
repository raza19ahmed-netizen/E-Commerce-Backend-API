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
public class RefundController : ControllerBase
{
    private readonly IRefundService _refundService;

    public RefundController(IRefundService refundService)
    {
        _refundService = refundService;
    }

    [HttpPost("payment/{paymentId}")]
    public async Task<IActionResult> CreateRefund(int paymentId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var refund = await _refundService
            .CreateRefundAsync(userId, paymentId);

        return Ok(new ApiResponse<RefundDto>(
            true,
            "Refund created successfully.",
            refund));
    }

    [HttpGet("payment/{paymentId}")]
    public async Task<IActionResult> GetRefund(int paymentId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var refund = await _refundService
            .GetRefundByPaymentIdAsync(userId, paymentId);

        if (refund == null)
        {
            return NotFound(new ApiResponse<object>(
                false,
                "Refund not found.",
                null));
        }

        return Ok(new ApiResponse<RefundDto>(
            true,
            "Refund retrieved successfully.",
            refund));
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAllRefunds()
    {
        var refunds = await _refundService.GetAllAsync();

        return Ok(new ApiResponse<IEnumerable<RefundDto>>(
            true,
            "All refund requests retrieved successfully.",
            refunds));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{refundId}/status")]
    public async Task<IActionResult> UpdateRefundStatus(
    int refundId,
    UpdateRefundStatusDto dto)
    {
        var refund = await _refundService
            .UpdateStatusAsync(refundId, dto);

        return Ok(new ApiResponse<RefundDto>(
            true,
            "Refund status updated successfully.",
            refund));
    }

}