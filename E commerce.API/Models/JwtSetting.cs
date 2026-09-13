namespace E_commerce.API.Models;

public class JwtSettings  // Developer ki soch "JWT ki saari settings ko alag-alag string/int variables ke roop me idhar-udhar use karne ke bajay ek JwtSettings object me rakh deta hoon//
{
    public string Key { get; set; } = string.Empty; // JWT ki secret key store karega//

    public string Issuer { get; set; } = string.Empty; // token kisne issue kiya....matlab E_CommerceAPI ne//
     
    public string Audience { get; set; } = string.Empty; // Token kisliye hai...matlab E_CommerceClient ke liye//

    public int ExpiresInMinutes { get; set; } // token kitne minute tak valid rahega//
}