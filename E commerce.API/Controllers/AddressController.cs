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
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    // ➕ Add new address
    [HttpPost]
    public async Task<IActionResult> CreateAddress(
        CreateAddressDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var address = await _addressService
            .CreateAddressAsync(userId, dto);

        return Ok(new ApiResponse<AddressDto>(
            true,
            "Address created successfully.",
            address));
    }

    // 📋 Get my all addresses
    [HttpGet]
    public async Task<IActionResult> GetMyAddresses()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var addresses = await _addressService
            .GetMyAddressesAsync(userId);

        return Ok(new ApiResponse<IEnumerable<AddressDto>>(
            true,
            "Addresses retrieved successfully.",
            addresses));
    }

    // 🔍 Get my address by ID
    [HttpGet("{addressId}")]
    public async Task<IActionResult> GetMyAddressById(
        int addressId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var address = await _addressService
            .GetMyAddressByIdAsync(userId, addressId);

        return Ok(new ApiResponse<AddressDto>(
            true,
            "Address retrieved successfully.",
            address));
    }

    // ✏️ Update my address
    [HttpPut("{addressId}")]
    public async Task<IActionResult> UpdateAddress(
        int addressId,
        UpdateAddressDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var address = await _addressService
            .UpdateAddressAsync(userId, addressId, dto);

        return Ok(new ApiResponse<AddressDto>(
            true,
            "Address updated successfully.",
            address));
    }

    // 🗑️ Delete my address
    [HttpDelete("{addressId}")]
    public async Task<IActionResult> DeleteAddress(
        int addressId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _addressService.DeleteAddressAsync(
            userId,
            addressId);

        return Ok(new ApiResponse<object>(
            true,
            "Address deleted successfully.",
            null));
    }

    // ⭐ Set default address
    [HttpPut("{addressId}/default")]
    public async Task<IActionResult> SetDefaultAddress(
        int addressId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var address = await _addressService
            .SetDefaultAddressAsync(userId, addressId);

        return Ok(new ApiResponse<AddressDto>(
            true,
            "Default address updated successfully.",
            address));
    }
}