namespace E_commerce.API.DTOs;

public class RefundDto
{
    public int Id { get; set; }

    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? RefundTransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RefundedAt { get; set; }
}