namespace E_commerce.API.Models;

public class WishlistItem
{
    public int Id { get; set; }

    // Wishlist item किस User का है
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    // कौन सा Product wishlist में है
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    // कब Wishlist में add किया गया
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}