using System.Security.Claims;
using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReturnController : ControllerBase
{
    private readonly IReturnService _returnService;

    public ReturnController(IReturnService returnService)
    {
        _returnService = returnService;
    }

    [HttpPost("order/{orderId}")]
    public async Task<IActionResult> CreateReturn(
        int orderId,
        CreateReturnRequestDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var returnRequest =
            await _returnService.CreateReturnAsync(
                userId,
                orderId,
                dto);

        return Ok(new ApiResponse<ReturnRequestDto>(
            true,
            "Return request created successfully.",
            returnRequest));
    }

    [HttpGet]
    public async Task<IActionResult> GetMyReturns()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var returns =
            await _returnService.GetMyReturnsAsync(userId);

        return Ok(new ApiResponse<IEnumerable<ReturnRequestDto>>(
            true,
            "Returns retrieved successfully.",
            returns));
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetReturnByOrderId(
        int orderId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var returnRequest =
            await _returnService.GetReturnByOrderIdAsync(
                userId,
                orderId);

        if (returnRequest == null)
        {
            return NotFound(
                new ApiResponse<ReturnRequestDto>(
                    false,
                    "Return request not found.",
                    null));
        }

        return Ok(new ApiResponse<ReturnRequestDto>(
            true,
            "Return request retrieved successfully.",
            returnRequest));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{returnId}/status")]
    public async Task<IActionResult> UpdateStatus(
        int returnId,
        UpdateReturnStatusDto dto)
    {
        var returnRequest =
            await _returnService.UpdateStatusAsync(
                returnId,
                dto);

        return Ok(new ApiResponse<ReturnRequestDto>(
            true,
            "Return status updated successfully.",
            returnRequest));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAllReturns()
    {
        var returns = await _returnService.GetAllAsync();

        return Ok(new ApiResponse<IEnumerable<ReturnRequestDto>>(
            true,
            "All return requests retrieved successfully.",
            returns));
    }
}