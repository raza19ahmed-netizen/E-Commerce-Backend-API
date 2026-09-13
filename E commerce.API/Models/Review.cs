namespace E_commerce.API.Models;

public class Review
{
    public int Id { get; set; }

    // किस Product का Review है
    public int ProductId { get; set; }

    // पूरा Product object access करने के लिए Navigation Property
    public Product Product { get; set; } = null!;//Nullable Reference Types enabled hone par compiler warning de sakta hai ki non-nullable Product property ko null assign kiya ja raha hai

    // किस User ने Review लिखा है
    public int UserId { get; set; }

    // पूरा User object access करने के लिए Navigation Property
    public User User { get; set; } = null!;//Nullable Reference Types enabled hone par compiler warning de sakta hai ki non-nullable Product property ko null assign kiya ja raha hai

    // Rating 1 से 5 तक होगी
    public int Rating { get; set; }

    // Customer का लिखा हुआ Review
    public string Comment { get; set; } = string.Empty;

    // Review कब लिखा गया
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}