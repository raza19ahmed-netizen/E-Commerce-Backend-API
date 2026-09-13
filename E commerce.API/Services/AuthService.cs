using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.AspNetCore.Identity; // isse passwordHasher<user> milta hai//
using Microsoft.Extensions.Options; // Ioptions<jwtSetting> ke through ye setting service me milsakti hai//
using Microsoft.IdentityModel.Tokens; // Jwt ko sign/secure karne ke liye important class deta hai//
using System.IdentityModel.Tokens.Jwt;// isse jwt banane vali class milti hai//
using System.Security.Claims; // jwt ke andar user ki information rakhne ke liye Claims use hote hain//
using System.Text; // jwt ki secret key ko byte me convert karne ke liye commanly use hota hai// 
namespace E_commerce.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly JwtSettings _jwtSettings;
    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings) // developer ko database me User search/save karna hai. isliye wo IUserRepository ko apne Service ke andar rakhta hai.//
    {                                                                                    //mjhe configuration ki JwtSetting wali value ek Strongly-typed object ke form me do//

        _userRepository = userRepository;
        _passwordHasher = new PasswordHasher<User>();// pasword ko hash karne wale object bnado..User model ke password ko hash/verify karne ke liye PasswordHasher use karo//
        _jwtSettings = jwtSettings.Value; // yahan actual JwtSetting Object nikala ja raha hai //
    }

    public async Task RegisterAsync(RegisterDto dto) // registration karne ka kaam karo aur client se aaya Registration deta dto me lo//
    {
        var existingUser = await _userRepository
            .GetByEmailAsync(dto.Email); // pehle data base me check karo ki is email se koi User already registered hai ya nahi//

        if (existingUser != null) // developer pooch raha hai kya email database me pehle se exist hai//
        {
            throw new InvalidOperationException(
                "User with this email already exists.");
        }

        var user = new User // theek hai ,email kisi existing user ka nhi hai. ab naya user object bnao//
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = "User"
            
       };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);// client ne jo original password bheja hai, use directlt database me save nhi karna pahle hash karo//

        await _userRepository.AddAsync(user); // user object ready hai , ab repository ko do aur database me save karwao//
    }
    public async Task<string> LoginAsync(LoginDto dto) //clent email/password bhejrha hai pehle emailse user dhoondo fir user nhi mila to login reject,agar mila to password verify karo, pasword galat to reject ,user ki information claims me rakkho,secret try se signing ki banao,jwt token banao,token ko string bana kar client ko return karo//
    {
        var user = await _userRepository
            .GetByEmailAsync(dto.Email); //pahle data base me is email ka user dhoondo//

        if (user == null)  // kya is email ka koi user mila?//
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");// agar nhi mila to loin reject//
        }
        // user milgaya tab ye chalega//
        var result = _passwordHasher.VerifyHashedPassword( //ya password verify hoga//
            user, // current user object//
            user.PasswordHash, //databse me save hashpassword//
            dto.Password); // login waqt user me jo original password type kiya//

        if (result == PasswordVerificationResult.Failed) // kya entred password galat nikla//
        {
            throw new UnauthorizedAccessException(  // login reject//
                "Invalid email or password.");
        }
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ye jwt ke andar user ki identity/information rakh raha hai  ...user.Id.Tostring() user ki id token me jarhi hai//
        new Claim(ClaimTypes.Name, user.Name),    // token me user ka naam //
        new Claim(ClaimTypes.Email, user.Email), // token me email //
        new Claim(ClaimTypes.Role, user.Role)   // token me role user ya admin //
    };

        var key = new SymmetricSecurityKey( //  symmetric ka matlab ha JWT ko sign aur verify karne ke liye same secret key use hoti hai//
            Encoding.UTF8.GetBytes(_jwtSettings.Key)); // apke appsetting.json me jwt:key hai Program us key ko JWT signing ke liye usable security key me convert kar raha hai//

        var credentials = new SigningCredentials( // ye credential jwt ko digital sign karne ke kaam aayenge.//
            key,
            SecurityAlgorithms.HmacSha256);  // jwt ko sign karne ke liye ye key aur HmacSha256 algorithm use karo//

        var token = new JwtSecurityToken( // ab actual jwt token create karo//
            issuer: _jwtSettings.Issuer, // token kisne issue kiya//
            audience: _jwtSettings.Audience, // token kis client ka hai//
            claims: claims,  // abhi jo information bnayi thi vo (userid,name,email,role) jwt me jarhi hai//
            expires: DateTime.UtcNow.AddMinutes( // token abhi se kitne minutes baad expire hoga?//
                _jwtSettings.ExpiresInMinutes),
            signingCredentials: credentials); // jwt ko sign karne ke liye jo key+hmacsha256 ready kiya tha vo yahan use horha hai//

        return new JwtSecurityTokenHandler() // jwtSecurityToken abhi C# ka token object hai , client ko generaly jwt string format me chahiye//
            .WriteToken(token);
    }
    public async Task<UserDto> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return new UserDto  // yaha user model ko userDto me convert kiya jarha hai//
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task ChangePasswordAsync(
    int userId,
    ChangePasswordDto dto) // mjhe User ki ID aur password-change ki information do mai password change kardunga//
    {
        var user = await _userRepository.GetByIdAsync(userId); //  repositary tum database se user nikalo//

        if (user == null) // kya database me user mila//
        {
            throw new KeyNotFoundException("User not found.");
        }

        var result = _passwordHasher.VerifyHashedPassword( // ab user milgaya , password change karne se pahle confirm karo ki user ko apna current pasword  actaul pta hai//
            user, // current user object//
            user.PasswordHash, //database me store hashed password//
            dto.CurrentPassword); // client ne jo current pasword bheja hai//

        if (result == PasswordVerificationResult.Failed) // kya current password galat tha..?//
        {
            throw new UnauthorizedAccessException(
                "Current password is incorrect.");
        }
        // current password sahi hai . Ab naye password ko Hash karke User Ke PasswordHash me replacce karo//
        user.PasswordHash = _passwordHasher.HashPassword( 
            user,
            dto.NewPassword);

        await _userRepository.UpdateAsync(user); // Is updated User ko database me bi save karo//
    }

    public async Task<string?> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        // user milne ke baad ab temperary password-reset token bnao//
        user.PasswordResetToken = Guid.NewGuid().ToString("N"); //ye apke User model ki property hai isme reset ke liye temporary token store kiya jayega...Guid.NewGuid() ye .net ka built in method hai jo ek unique Guid generate karta hai matlab reset password ke liye temporary token store kiya jayega ..fir use string me convert karega "N" ka matlab format me hypen hta deta hai//

        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15); // token password expire after 10:15 //

        await _userRepository.UpdateAsync(user); // save in data base//

        return user.PasswordResetToken;
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);// client ne user diya hai pehle database me is email ka user dhoondo//

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (string.IsNullOrEmpty(user.PasswordResetToken) || //ab verify karo ki client ke pass valid reset token hai  isme 2 condition hai....agar passwordResetToken = null...ya PasswordResetToken = "" to condition true matlab user lke pass reset token nhi hai//
            user.PasswordResetToken != dto.Token)  // data base token aur client ka token campare // 
        {
            throw new UnauthorizedAccessException(
                "Invalid reset token.");
        }

        if (!user.PasswordResetTokenExpiry.HasValue || // agar expiry date hai nhi, to token nhi manajayega //
            user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)// token ki expiry time abhi ke time se pehle ka hai?//
        {
            throw new UnauthorizedAccessException(
                "Reset token has expired.");
        }

        user.PasswordHash = _passwordHasher.HashPassword( // new password ko secure hash me convert karo //
            user,
            dto.NewPassword);

        user.PasswordResetToken = null; // ye reset token one time use tha ab ise invalid kardo//
        user.PasswordResetTokenExpiry = null; // Ab reset process complete hogya hai isliye expiry information bhi hta do //

        await _userRepository.UpdateAsync(user);// update in data base//
    }
}