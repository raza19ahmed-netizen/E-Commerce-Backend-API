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
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    private readonly IOrderRepository _orderRepository;

    public ShipmentController(
        IShipmentService shipmentService,
        IOrderRepository orderRepository)
    {
        _shipmentService = shipmentService;
        _orderRepository = orderRepository;
    }

    // 📦 Admin creates shipment
    [Authorize(Roles = "Admin")]
    [HttpPost("order/{orderId}")]
    public async Task<IActionResult> CreateShipment(
        int orderId,
        CreateShipmentDto dto)
    {
        var shipment = await _shipmentService
            .CreateShipmentAsync(orderId, dto);

        return Ok(new ApiResponse<ShipmentDto>(
            true,
            "Shipment created successfully.",
            shipment));
    }

    // 🔍 Get shipment by Shipment ID
    [HttpGet("{shipmentId}")]
    public async Task<IActionResult> GetShipmentById(int shipmentId)
    {
        var shipment = await _shipmentService
            .GetByIdAsync(shipmentId);

        // Security check:
        // Normal user can only view shipment of their own order
        if (!User.IsInRole("Admin"))
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _orderRepository
                .GetByIdAsync(shipment.OrderId);

            if (order == null || order.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to view this shipment.");
            }
        }

        return Ok(new ApiResponse<ShipmentDto>(
            true,
            "Shipment retrieved successfully.",
            shipment));
    }

    // 📦 Get shipment using Order ID
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetShipmentByOrderId(int orderId)
    {
        // Normal user ownership check
        if (!User.IsInRole("Admin"))
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            if (order.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to view this shipment.");
            }
        }

        var shipment = await _shipmentService
            .GetByOrderIdAsync(orderId);

        if (shipment == null)
        {
            return NotFound(new ApiResponse<object>(
                false,
                "Shipment not found.",
                null));
        }

        return Ok(new ApiResponse<ShipmentDto>(
            true,
            "Shipment retrieved successfully.",
            shipment));
    }

    // 🚚 Admin updates shipment status
    [Authorize(Roles = "Admin")]
    [HttpPut("{shipmentId}/status")]
    public async Task<IActionResult> UpdateShipmentStatus(
        int shipmentId,
        UpdateShipmentStatusDto dto)
    {
        var shipment = await _shipmentService
            .UpdateStatusAsync(shipmentId, dto);

        return Ok(new ApiResponse<ShipmentDto>(
            true,
            "Shipment status updated successfully.",
            shipment));
    }
}