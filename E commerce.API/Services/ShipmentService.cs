using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IOrderRepository _orderRepository;

    public ShipmentService(
        IShipmentRepository shipmentRepository,
        IOrderRepository orderRepository)
    {
        _shipmentRepository = shipmentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<ShipmentDto> CreateShipmentAsync(
        int orderId,
        CreateShipmentDto dto)
    {
        // Step 1: Order find karo
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        // Step 2: Check existing shipment
        var existingShipment =
            await _shipmentRepository.GetByOrderIdAsync(orderId);

        if (existingShipment != null)
        {
            throw new InvalidOperationException(
                "Shipment already exists for this order.");
        }

        // Step 3: Order status validation
        if (order.Status != "Confirmed")
        {
            throw new InvalidOperationException(
                "Shipment can only be created for confirmed orders.");
        }

        // Step 4: Create shipment
        var shipment = new Shipment
        {
            OrderId = orderId,
            TrackingNumber = dto.TrackingNumber.Trim(),
            Status = "Processing"
        };

        var savedShipment =
            await _shipmentRepository.AddAsync(shipment);

        return MapToDto(savedShipment);
    }

    public async Task<ShipmentDto> GetByIdAsync(int shipmentId)
    {
        var shipment =
            await _shipmentRepository.GetByIdAsync(shipmentId);

        if (shipment == null)
            throw new KeyNotFoundException("Shipment not found.");

        return MapToDto(shipment);
    }

    public async Task<ShipmentDto?> GetByOrderIdAsync(int orderId)
    {
        var shipment =
            await _shipmentRepository.GetByOrderIdAsync(orderId);

        if (shipment == null)
            return null;

        return MapToDto(shipment);
    }

    public async Task<ShipmentDto> UpdateStatusAsync(
        int shipmentId,
        UpdateShipmentStatusDto dto)
    {
        var shipment =
            await _shipmentRepository.GetByIdAsync(shipmentId);

        if (shipment == null)
            throw new KeyNotFoundException("Shipment not found.");

        var newStatus = dto.Status.Trim();

        // Valid status transitions
        var isValidTransition =
            (shipment.Status == "Processing" &&
             newStatus == "Shipped")

            ||

            (shipment.Status == "Shipped" &&
             newStatus == "OutForDelivery")

            ||

            (shipment.Status == "OutForDelivery" &&
             newStatus == "Delivered");

        if (!isValidTransition)
        {
            throw new InvalidOperationException(
                $"Invalid shipment status transition from " +
                $"{shipment.Status} to {newStatus}.");
        }

        shipment.Status = newStatus;

        // Shipped time
        if (newStatus == "Shipped")
        {
            shipment.ShippedAt = DateTime.UtcNow;
        }

        // Delivered time + Order update
        if (newStatus == "Delivered")
        {
            shipment.DeliveredAt = DateTime.UtcNow;

            if (shipment.Order != null)
            {
                shipment.Order.Status = "Delivered";
            }
        }

        await _shipmentRepository.UpdateAsync(shipment);

        return MapToDto(shipment);
    }

    private static ShipmentDto MapToDto(Shipment shipment)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,
            OrderId = shipment.OrderId,
            TrackingNumber = shipment.TrackingNumber,
            Status = shipment.Status,
            CreatedAt = shipment.CreatedAt,
            ShippedAt = shipment.ShippedAt,
            DeliveredAt = shipment.DeliveredAt
        };
    }
}