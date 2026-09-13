namespace E_commerce.API.DTOs;

public class ReviewDto //review banjane ke baad user ko kya kya dikahna hai
{
    public int Id { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    // Review किसने दिया उसका नाम
    public string UserName { get; set; } = string.Empty;
}