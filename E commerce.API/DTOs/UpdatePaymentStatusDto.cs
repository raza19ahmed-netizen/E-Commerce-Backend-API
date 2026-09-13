namespace E_commerce.API.DTOs;

public class UpdatePaymentStatusDto
{
    public string Status { get; set; } = string.Empty;

    public string? TransactionId { get; set; }
}