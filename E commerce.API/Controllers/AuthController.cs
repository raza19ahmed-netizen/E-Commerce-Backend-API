using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace E_commerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase  //
{
    private readonly IAuthService _authService; // developer direct AuthService ko use nhi kar raha vo interface use kar raha hai//

    public AuthController(IAuthService authService) // controller ko jab bnaya jaye to use Authservice dedena//
    {
        _authService = authService;
    }
     
    [HttpPost("register")]     // AuthController me post request/register ke liye ye method chalana//
    public async Task<IActionResult> Register(RegisterDto dto) // client se registration ka data RegisterDto ke form me lao aur registration operation perform karo//
    {
        await _authService.RegisterAsync(dto); // controller yaha khud register nhi kr raha vo service ko boraha ..ye registration data hai , is user ko register kardo//

        return Ok(new ApiResponse<object>(
            true,
            "User registered successfully.",
            null));
    }
    [HttpPost("login")] // Post request /login par ayegi to neeche wala Login() method execute karo//
    public async Task<IActionResult> Login(LoginDto dto) // Asp.net core clint ke JSON ko LoginDto Object me convert karega  .. dto.Email....dto.pasword//
    {
        var token = await _authService.LoginAsync(dto);// controller service se bolraha hai ye Logindetail hain,check karo user valid hai ki nahi// 
        return Ok(new ApiResponse<object>( //login succesfull hai ,ab client ko Http 200 response bhejo//
             true,
            "Login successful.",
            new
            {
                token
                // user.Id,
                //user.Name,
                //user.Email,
                //user.Role
            }));  // in selected value ko lekar ek temprary object bnado taki wahi 4 property Api response me bheji jasaken. iski anonymous object kahte hai//
    }

    [Authorize]   // kya request ke sath valid JWT token hai//
    [HttpGet("me")] // jo user khud login hai uski infomation do//
    public async Task<IActionResult> GetCurrentUser() // ye current logged-in-user ka profile return karega...developer ki basic thinking Jwt token me maine login ke time User ki Id NameIdentifier claim ke andar rkhi thi. Ab /me Api se mjhe vahi Id token se nikalni hai,string se int me convert karni ahai aur userId varaible me rakhni hai//
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!); // ya pe "User" actualy controllerBase ke andar se milne wali property hai. aur ye user cuurrent HTTP request ke Authenticated user ki information rakta hai...FindFisrtValue() method User ke andar Claims me se kisi Particular cliam kivalue dhoonta hai..developer isliye User ke claims me se mujhe NameIdentifier wali value Chahiye. ! iska matlb developer compiler se khe ha mjhe pta hai ye value null nhi hogi mjhe warning na do//
        var user = await _authService.GetCurrentUserAsync(userId); // user ke andar ye poora UserDto object hai //

        return Ok(new ApiResponse<UserDto>( // yaha developer ek APiresponce Object bna raha hai ..is ApiResponse ke data andar UserDto type ka data hoga  //
            true,
            "User profile retrieved successfully.",
            user)); // jaha apne Apiresponse  bnaya hai vaha se data ayega //
    
    }

    [Authorize] //ye Api sirf logged in user use karsakta hai//
    [HttpPost("change-password")] // ye Api endpoint define karta hai //
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _authService.ChangePasswordAsync(userId, dto); //userId ka password dto ke according change karo//

        return Ok(new ApiResponse<object>(
            true,
            "Password changed successfully.",
            null));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
       var token = await _authService.ForgotPasswordAsync(dto.Email);

        return Ok(new ApiResponse<object>(
            true,
            "Password reset token generate successfully.",
            new
        {
            token
        }));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        await _authService.ResetPasswordAsync(dto);

        return Ok(new ApiResponse<object>(
            true,
            "Password reset successfully.",
            null));
    }
}