using E_commerce.API.DTOs;

namespace E_commerce.API.Interfaces;

public interface IAddressService
{
    Task<AddressDto> CreateAddressAsync(
        int userId,
        CreateAddressDto dto);

    Task<IEnumerable<AddressDto>> GetMyAddressesAsync(
        int userId);

    Task<AddressDto> GetMyAddressByIdAsync(
        int userId,
        int addressId);

    Task<AddressDto> UpdateAddressAsync(
        int userId,
        int addressId,
        UpdateAddressDto dto);

    Task DeleteAddressAsync(
        int userId,
        int addressId);

    Task<AddressDto> SetDefaultAddressAsync(
        int userId,
        int addressId);
}