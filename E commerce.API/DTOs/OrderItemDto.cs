namespace E_commerce.API.DTOs;

public class OrderItemDto 
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    // इस particular item की कुल कीमत
    public decimal TotalPrice => Quantity * UnitPrice;
}