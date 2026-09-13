namespace E_commerce.API.Models;

public class Refund
{
    public int Id { get; set; }

    public int PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = "Pending";

    public string? RefundTransactionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RefundedAt { get; set; }
}