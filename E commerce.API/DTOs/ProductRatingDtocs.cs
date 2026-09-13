namespace E_commerce.API.DTOs;

public class ProductRatingDto
{
    public int ProductId { get; set; }

    public double AverageRating { get; set; }

    public int TotalReviews { get; set; }
}