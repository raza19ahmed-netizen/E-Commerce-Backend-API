namespace E_commerce.API.DTOs;

public class ReturnRequestDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }
}