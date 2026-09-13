using E_commerce.API.DTOs;
using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IAuthService  // mere Authentication System me registration ka ek operation hona Chahiye //
{
    Task RegisterAsync(RegisterDto dto);  // user ko regiter karne ka poora business logic yaha hoga //
                                          // Task <User> LoginAsync(LoginDto dto);
    Task<string> LoginAsync(LoginDto dto); // client se LoginDto lo, login verify karo, aur successful login par JWT token string return karo//
    Task<UserDto> GetCurrentUserAsync(int userId);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<string?> ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(ResetPasswordDto dto);

}