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
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var order = await _orderService.CheckoutAsync(userId, dto);

        return Ok(new ApiResponse<OrderDto>(
            true,
            "Order placed successfully.",
            order));
    }
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        // JWT Token से currently logged-in user की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // सिर्फ इसी user के orders प्राप्त करो
        var orders = await _orderService.GetMyOrdersAsync(userId);

        return Ok(new ApiResponse<IEnumerable<OrderDto>>(
            true,
            "Orders retrieved successfully.",
            orders));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMyOrderById(int id)
    {
        // JWT Token से logged-in user की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // सिर्फ इसी user का specific order प्राप्त करो
        var order = await _orderService
            .GetMyOrderByIdAsync(id, userId);

        return Ok(new ApiResponse<OrderDto>(
            true,
            "Order retrieved successfully.",
            order));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
    int id,
    UpdateOrderStatusDto dto)
    {
        var updatedOrder = await _orderService
            .UpdateOrderStatusAsync(id, dto);

        return Ok(new ApiResponse<OrderDto>(
            true,
            "Order status updated successfully.",
            updatedOrder));
    }

    [HttpPut("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var order = await _orderService
            .CancelOrderAsync(userId, orderId);

        return Ok(new ApiResponse<OrderDto>(
            true,
            "Order cancelled successfully.",
            order));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{id}")]
    public async Task<IActionResult> GetOrderByIdForAdmin(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        return Ok(new ApiResponse<OrderDto>(
            true,
            "Order retrieved successfully.",
            order));
    }
}