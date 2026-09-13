using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; //Controller/API ke features chahiye, jaise ControllerBase, IActionResult, [HttpPost].
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace E_commerce.API.Controllers;

[ApiController]             // Ye normal class nahi, API Controller hai.
[Route("api/[controller]")] // Is controller ke endpoints ka base URL api/Cart hoga.
[Authorize]                 // Cart ke endpoints sirf logged-in/authenticated user access kare.
public class CartController : ControllerBase
{
    private readonly ICartService _cartService; // Controller khud cart ka business logic nahi karega. CartService ko responsibility do."

    public CartController(ICartService cartService)
    {
        _cartService = cartService; // CartController ko CartService chahiye.
    }

    [HttpPost("items")] //"Client jab POST request /items par bheje to niche vala method execute karo//
    public async Task<IActionResult> AddToCart(AddToCartDto dto)//Client ne cart me product add karne ki request bheji hai, us request ko receive karo."
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);//Client ne request bheji hai. Mujhe pata karna hai ki ye request kis logged-in user ki hai."

        await _cartService.AddToCartAsync(userId, dto);//"Service, User 5 ke cart me DTO ke according product add kar do."

        return Ok(new ApiResponse<object>(//Service ka kaam successfully complete ho gaya, ab client ko success response bhejo."
            true,
            "Product added to cart successfully.",
            null));
    }//Client cart me product add karne ki request bhejega. Controller request receive karega, JWT se pata karega ki kaunsa user request kar raha hai, phir CartService ko userId + product data dega. Controller khud business logic/database ka kaam nahi karega."

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var cart = await _cartService.GetCartAsync(userId);

        return Ok(new ApiResponse<CartDto>(
            true,
            "Cart retrieved successfully.",
            cart));
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _cartService.RemoveFromCartAsync(userId, productId);

        return Ok(new ApiResponse<object>(
            true,
            "Product removed from cart successfully.",
            null));
    }

    [HttpPut("items/{productId}")]//Client productId aur new quantity bheje → logged-in user ka ID JWT se nikalo → Service ko ye 3 cheezein do → quantity update karao → success response bhejo."
    public async Task<IActionResult> UpdateCartItem(
    int productId,
    UpdateCartItemDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!); //User...Ye Controller ka built-in property hai. Isme currently logged -in/ authenticated user ki information hoti hai.


        await _cartService.UpdateCartItemAsync( // developer service ko 3 information derha hai
            userId,    // kis user ka caet hai
            productId, // kaunsa product?
            dto);      // new quantity kya hai?

        return Ok(new ApiResponse<object>(
            true,
            "Cart item quantity updated successfully.",
            null));
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _cartService.ClearCartAsync(userId);

        return Ok(new ApiResponse<object>(
            true,
            "Cart cleared successfully.",
            null));
    }
}