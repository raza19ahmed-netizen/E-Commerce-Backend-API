using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;

    public AddressService(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<AddressDto> CreateAddressAsync(
        int userId,
        CreateAddressDto dto)
    {
        // User ke existing addresses check karo
        var existingAddresses =
            await _addressRepository.GetByUserIdAsync(userId);//Naya address save karne se pehle mujhe check karna hai ki user ke paas pehle se address hai ya nahi."

        // Agar ye user ka first address hai to automatically default banao
        var isDefault = !existingAddresses.Any(); //.Any() matlam collection me kamse ek item hai...! iska matlab NOT...agar koi address nhi hai to first address defoult rahrgi

        var address = new Address
        {
            UserId = userId,
            FullName = dto.FullName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            AddressLine = dto.AddressLine.Trim(),
            City = dto.City.Trim(),
            State = dto.State.Trim(),
            PostalCode = dto.PostalCode.Trim(),
            Country = dto.Country.Trim(),
            IsDefault = isDefault
        };

        var savedAddress =
            await _addressRepository.AddAsync(address);

        return MapToDto(savedAddress);
    }

    public async Task<IEnumerable<AddressDto>> GetMyAddressesAsync(
        int userId)
    {
        var addresses =
            await _addressRepository.GetByUserIdAsync(userId);

        return addresses.Select(MapToDto).ToList();//"addresses collection ke har Address object par MapToDto method chalao."
    }

    public async Task<AddressDto> GetMyAddressByIdAsync(
        int userId,
        int addressId)
    {
        var address =
            await _addressRepository.GetByIdAsync(addressId);

        if (address == null)
        {
            throw new KeyNotFoundException("Address not found.");
        }

        // Ownership check
        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to access this address.");
        }

        return MapToDto(address);
    }

    public async Task<AddressDto> UpdateAddressAsync(
        int userId,
        int addressId,
        UpdateAddressDto dto)
    {
        var address =
            await _addressRepository.GetByIdAsync(addressId);

        if (address == null)
        {
            throw new KeyNotFoundException("Address not found.");
        }

        // Ownership check
        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to update this address.");
        }

        address.FullName = dto.FullName.Trim();
        address.PhoneNumber = dto.PhoneNumber.Trim();
        address.AddressLine = dto.AddressLine.Trim();
        address.City = dto.City.Trim();
        address.State = dto.State.Trim();
        address.PostalCode = dto.PostalCode.Trim();
        address.Country = dto.Country.Trim();

        await _addressRepository.UpdateAsync(address);

        return MapToDto(address);
    }

    public async Task DeleteAddressAsync(
        int userId,
        int addressId)
    {
        var address =
            await _addressRepository.GetByIdAsync(addressId);

        if (address == null)
        {
            throw new KeyNotFoundException("Address not found.");
        }

        // Ownership check
        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to delete this address.");
        }

        // Important: Default address delete hone par
        // agar aur addresses hain to unme se ek ko default bana denge
        var wasDefault = address.IsDefault;//Kya jo address delete hone wala hai, wo default tha?"

        await _addressRepository.DeleteAsync(address);

        if (wasDefault)//Agar humne default address delete kiya hai...To remaining addresses lao
        {
           
            
            var remainingAddresses =
                await _addressRepository.GetByUserIdAsync(userId);

            var newDefault = remainingAddresses.FirstOrDefault();// Remaining addresses me se pehla address le lo

            if (newDefault != null)
            {
                newDefault.IsDefault = true;
                
                await _addressRepository.UpdateAsync(newDefault);//Ab automatically koi doosra address default ban gay
            }
        }
    }

    public async Task<AddressDto> SetDefaultAddressAsync(//User jis address ko choose kare, usko default bana do.
        int userId,
        int addressId)
    {
        var address =
            await _addressRepository.GetByIdAsync(addressId);

        if (address == null)
        {
            throw new KeyNotFoundException("Address not found.");
        }

        // Ownership check
        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to modify this address.");
        }

        // User ke saare addresses lao
        var addresses =
            await _addressRepository.GetByUserIdAsync(userId);

        // Purane default ko remove karo
        foreach (var item in addresses)
        {
            item.IsDefault = false;//Pehle ensure karo ki kisi bhi address ka default status true na rahe."
        }

        // Selected address ko default banao
        address.IsDefault = true;

        // Selected + baaki addresses update
        foreach (var item in addresses)
        {
            await _addressRepository.UpdateAsync(item);
        }

        return MapToDto(address);
    }

    // Entity → DTO conversion helper
    private static AddressDto MapToDto(Address address)
    {
        return new AddressDto
        {
            Id = address.Id,
            FullName = address.FullName,
            PhoneNumber = address.PhoneNumber,
            AddressLine = address.AddressLine,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            IsDefault = address.IsDefault
        };
    }
}