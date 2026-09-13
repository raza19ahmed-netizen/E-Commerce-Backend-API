namespace E_commerce.API.Models;

public class Cart // har logged-in-user ka ek cart hoga aur us cart ke andar multiple cartitem honge//
{
    public int Id { get; set; } //cart ki unique id//

    public int UserId { get; set; } // ye cart kis user ka hai  ..use as foreign key //

    public User User { get; set; } = null!; // user ka object...ye database me null save karne ke liye nhi hai//

    public ICollection<CartItem> CartItems { get; set; }  // ek cart ke andar multiple cartitem hosakte hai//
        = new List<CartItem>(); // developer yaha empty collection bna rha hai... matlab naya cart create hua// 
}
