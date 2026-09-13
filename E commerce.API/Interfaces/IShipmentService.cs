using E_commerce.API.DTOs;

namespace E_commerce.API.Interfaces;

public interface IShipmentService
{
    // Admin shipment create करेगा
    Task<ShipmentDto> CreateShipmentAsync(
        int orderId,
        CreateShipmentDto dto);

    // User/Admin shipment ID से देख सकेगा
    Task<ShipmentDto> GetByIdAsync(int shipmentId);

    // किसी Order की shipment देखना
    Task<ShipmentDto?> GetByOrderIdAsync(int orderId);

    // Admin shipment status update करेगा
    Task<ShipmentDto> UpdateStatusAsync(
        int shipmentId,
        UpdateShipmentStatusDto dto);
}