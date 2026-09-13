namespace E_commerce.API.Models;

public class CartItem // cart me kaunsa product hai, kitne quanrity hai, aur ye item kis cartka hai..ye sb mjhe info mjhe CartItem me rakhni hai//
{
    public int Id { get; set; } // cartitem ki id//

    public int CartId { get; set; } // ye cartitem kis cart ke andar hai

    public Cart Cart { get; set; } = null!; //ye cartid ka navigation property hai...matlb actaul caert object//

    public int ProductId { get; set; } //cart me konsa product add kiya gaya hai?//

    public Product Product { get; set; } = null!; // actual product object//

    public int Quantity { get; set; } //customer ne product kitne qauntity me add kiya hai//
}