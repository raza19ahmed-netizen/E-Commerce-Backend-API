using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IShipmentRepository
{
    Task<Shipment> AddAsync(Shipment shipment);

    Task<Shipment?> GetByIdAsync(int id);

    Task<Shipment?> GetByOrderIdAsync(int orderId);

    Task UpdateAsync(Shipment shipment);
}