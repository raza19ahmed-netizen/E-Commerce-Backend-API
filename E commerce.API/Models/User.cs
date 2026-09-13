namespace E_commerce.API.Models;

public class User
{
    public int Id { get; set; } //har user ki unique identity//

    public string Name { get; set; } = string.Empty; //user name//

    public string Email { get; set; } = string.Empty; //User ka email/login identity//

    public string PasswordHash { get; set; } = string.Empty; // password ko direct nhi hash form me insert karna//

    public string Role { get; set; } = "User"; // user normal hai ya Admin hai..etc  defoult name user // 
    public string? PasswordResetToken {  get; set; } //temperory secret token//
    public DateTime? PasswordResetTokenExpiry { get; set; } //token kab expre hoga

    public ICollection<Order> Orders { get; set; }
    = new List<Order>();

}
