using E_commerce.API.DTOs;//"Mujhe is interface me DTO classes use karni hain, isliye DTOs namespace ko import kar

namespace E_commerce.API.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CheckoutAsync(int userId,CheckoutDto dto); // "User cart se checkout karega aur Order create hoga."

    Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId);//"Logged-in user ko uske saare orders dikhane hain."

    Task<OrderDto> GetMyOrderByIdAsync(int orderId, int userId);//User apne particular order ko ID se dekhna chahta hai."

    Task<OrderDto> GetOrderByIdAsync(int orderId);

    Task<OrderDto> UpdateOrderStatusAsync(//Existing Order ka status change karna hai
    int orderId, //"Mujhe batao kis Order ka status change karna hai
    UpdateOrderStatusDto dto);//Yahan actual new status aayega.

    Task<OrderDto> CancelOrderAsync(int userId, int orderId);




}

