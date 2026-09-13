namespace E_commerce.API.Models;

public class Payment
{
    public int Id { get; set; }

    // Payment किस Order के लिए है
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    // Payment की amount
    public decimal Amount { get; set; }

    // Payment की current स्थिति
    // Pending, Success, Failed
    public string Status { get; set; } = "Pending";

    // Gateway का transaction/payment ID
    public string? TransactionId { get; set; }

    // Payment कब create हुआ
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Payment कब complete हुआ
    public DateTime? PaidAt { get; set; }

    public string PaymentMethod { get; set; } = "Online";
}
