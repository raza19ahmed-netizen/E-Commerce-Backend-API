namespace E_commerce.API.DTOs;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new(); //List ka matlb mjhe cart me multiple item rakhne hai, cart ke andar mobile,laptop,mouse etc,..<CartItemDto> ka matlab list ke andar kis type ke object honge?..Items means Caert ke andar jo product/items hain, unko Items naam se access karunga
                                                            // new() means jab CartDto create ho, items ki initially empty List bana do.
    public decimal TotalAmount =>  // client ko poore Cart ki total price bhi deni hai
        Items.Sum(item => item.TotalPrice); // Item ke andar sabhi items ki value add karo,Items ko ek ek karke dekho aur har item ki TotalPrice nikaalo.
}