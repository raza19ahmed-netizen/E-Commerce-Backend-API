namespace E_commerce.API.DTOs;

public class UpdateRefundStatusDto
{
    public string Status { get; set; } = string.Empty;

    public string? RefundTransactionId { get; set; }
}
