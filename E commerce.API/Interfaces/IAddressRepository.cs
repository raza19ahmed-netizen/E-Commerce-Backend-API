using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IAddressRepository
{
    Task<Address> AddAsync(Address address);

    Task<IEnumerable<Address>> GetByUserIdAsync(int userId);

    Task<Address?> GetByIdAsync(int id);

    Task UpdateAsync(Address address);

    Task DeleteAsync(Address address);
}