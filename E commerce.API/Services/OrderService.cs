using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using E_commerce.API.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_commerce.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository; //Order ko database me save/update/read karne ke liye OrderRepository chahiye
    private readonly ICartRepository _cartRepository; // Checkout se pehle user ki cart aur uske items chahiye."
    private readonly IProductRepository _productRepository; // Product ki latest price aur stock database se check karna hai."
    private readonly ApplicationDbContext _context; //Mujhe database-level transaction directly start karni hai."
    private readonly IRefundService _refundService;
    public readonly ICouponRepository _couponRepository;
    private readonly ICouponUsageRepository _couponUsageRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        ApplicationDbContext context,
        IRefundService refundService,
        ICouponRepository couponRepository,
        ICouponUsageRepository couponUsageRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _context = context;
        _refundService = refundService;
        _couponRepository = couponRepository;
        _couponUsageRepository = couponUsageRepository;
    }

    public async Task<OrderDto> CheckoutAsync(int userId ,CheckoutDto dto) //"User checkout kar raha hai. Mujhe us user ki cart lekar actual Order banana hai aur final me OrderDto return karna hai."
    {
        // Step 1: User की Cart उसके सभी Items के साथ प्राप्त करो
        var cart = await _cartRepository.GetCartWithItemsAsync(userId);

        // Step 2: Cart मौजूद नहीं है या खाली है तो Checkout नहीं हो सकता
        if (cart == null || !cart.CartItems.Any())
        {
            throw new InvalidOperationException(
                "Cannot checkout because your cart is empty.");
        }

        //  Database Transaction शुरू ,"Ab checkout me multiple database changes hone wale hain. In sabko ek transaction me rakho."
        await using var transaction =
           await _context.Database.BeginTransactionAsync();// ye EF ka inbuild method hai...database transaction start  karo,  uske ready hone ka wait karo,transaction object ko transaction naam se rakho,aur kaam khtam hone pr us rersource ko automaticly clean kardo

        try   //Ab risky database operations start ho rahe hain. Agar koi error aaye to catch me rollback karunga.
        {

            Coupon? coupon = null; //Ye variable Coupon object bhi hold kar sakta hai aur null bhi ho sakta hai.

            if (!string.IsNullOrWhiteSpace(dto.CouponCode))//CouponCode null/empty/space nahi hai.
            {
                var couponCode = dto.CouponCode.Trim().ToUpper();

                coupon = await _couponRepository.GetByCodeAsync(couponCode);

                if (coupon == null)
                {
                    throw new KeyNotFoundException("Invalid coupon code.");
                }

                if (!coupon.IsActive)
                {
                    throw new InvalidOperationException("This coupon is not active.");
                }

                if (DateTime.UtcNow < coupon.StartDate)
                {
                    throw new InvalidOperationException(
                        "This coupon is not valid yet.");
                }

                if (DateTime.UtcNow > coupon.ExpiryDate)
                {
                    throw new InvalidOperationException(
                        "This coupon has expired.");
                }

                if (coupon.UsedCount >= coupon.UsageLimit)
                {
                    throw new InvalidOperationException(
                        "Coupon usage limit has been reached.");
                }

                // Check करो कि इस user ने यह coupon पहले use किया है या नहीं
                var existingUsage = await _couponUsageRepository
                    .GetByUserAndCouponAsync(userId, coupon.Id);

                if (existingUsage != null && existingUsage.UsageCount >= 1)
                {
                    throw new InvalidOperationException(
                        "You have already used this coupon.");
                }
            }
            // Step 3: नया Order तैयार करो checkout ke liye,Checkout successful hone wala hai, isliye ek naya Order object prepare karo.
            var order = new Order
            {
                UserId = userId, // ye order kis userid ka ka hai? current login user
                OrderDate = DateTime.UtcNow,
                Status = "Pending" //Abhi payment/shipping process complete nahi hua, isliye Pending.
            };

            foreach (var cartItem in cart.CartItems)//cart me jitne product hai,unko ek ek karke process karo
            {
                // Database से Product की latest information लो
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);//"Cart me purani information ho sakti hai. Checkout ke waqt database se Product ki latest information lao."

                if (product == null)//Cart me ProductId hai, lekin database me product exist nahi karta. Checkout rok do.
                {
                    throw new KeyNotFoundException(
                        $"Product with ID {cartItem.ProductId} not found.");
                }

                // Latest stock check करो
                if (cartItem.Quantity > product.Stock)//Customer jitna quantity order kar raha hai, utna stock available hai ya nahi?"
                {
                    throw new InvalidOperationException(
                        $"Only {product.Stock} items of {product.Name} are available in stock.");
                }

                // Stock कम करो
                product.Stock -= cartItem.Quantity;//suppose Customer ne 3 purchase kiye, inventory se 3 kam karo."

                // Updated Stock database में save करो
                await _productRepository.UpdateAsync(product);

                // Valid होने पर OrderItem बनाओ
                var orderItem = new OrderItem //Cart ka product ab actual Order ka item ban raha hai."
                {
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,

                    // हमेशा latest database price का snapshot लो
                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);//Ab is OrderItem ko current Order ke andar attach karo."
            }

            // Step 5: सभी OrderItems का Total Amount calculate करो
            order.TotalAmount = order.OrderItems
                .Sum(item => item.Quantity * item.UnitPrice);

            // 🎟️ Coupon लागू करो
            if (coupon != null)
            {
                // पहले check करो कि order minimum amount पूरा करता है या नहीं
                if (order.TotalAmount < coupon.MinimumOrderAmount)
                {
                    throw new InvalidOperationException(
                        $"Minimum order amount of {coupon.MinimumOrderAmount} is required to use this coupon.");
                }

                decimal discountAmount;

                // Percentage discount
                if (coupon.DiscountType == "Percentage")
                {
                    discountAmount =
                        order.TotalAmount * coupon.DiscountValue / 100;

                    // Maximum discount लागू करो, अगर दिया गया है
                    if (coupon.MaximumDiscountAmount.HasValue &&
                        discountAmount > coupon.MaximumDiscountAmount.Value)
                    {
                        discountAmount = coupon.MaximumDiscountAmount.Value;
                    }
                }
                else // Fixed discount
                {
                    discountAmount = coupon.DiscountValue;
                }

                // Discount order total से ज्यादा नहीं हो सकता
                if (discountAmount > order.TotalAmount)
                {
                    discountAmount = order.TotalAmount;
                }

                // Final amount
                order.TotalAmount -= discountAmount;

                // Coupon usage count बढ़ाओ
                coupon.UsedCount++;

                await _couponRepository.UpdateAsync(coupon);

                // 👤 इस user की coupon usage history save करो
                var couponUsage = await _couponUsageRepository
                    .GetByUserAndCouponAsync(userId, coupon.Id);

                if (couponUsage == null)
                {
                    // पहली बार इस user ने coupon use किया
                    couponUsage = new CouponUsage
                    {
                        UserId = userId,
                        CouponId = coupon.Id,
                        UsageCount = 1,
                        LastUsedAt = DateTime.UtcNow
                    };

                    await _couponUsageRepository.AddAsync(couponUsage);
                }
                else
                {
                    // अगर future में per-user limit बढ़ानी हो
                    couponUsage.UsageCount++;
                    couponUsage.LastUsedAt = DateTime.UtcNow;

                    await _couponUsageRepository.UpdateAsync(couponUsage);
                }
            }

            // तैयार Order और उसके Items को database में save करो
            var savedOrder = await _orderRepository.AddAsync(order);



            // Order successfully save होने के बाद Cart के सभी Items हटाओ
            await _cartRepository.ClearCartAsync(cart.Id);


            // Database से Order को उसके OrderItems और Products के साथ प्राप्त करो
            var completeOrder = await _orderRepository.GetByIdAsync(savedOrder.Id);

            if (completeOrder == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            // सभी operations successfully complete हो गए → Transaction Commit
            await transaction.CommitAsync();

            return new OrderDto//"Database ka complete Order object directly client ko nahi bhejna. Clean DTO bana kar response do.

            {
                Id = completeOrder.Id,
                OrderDate = completeOrder.OrderDate,
                TotalAmount = completeOrder.TotalAmount,
                Status = completeOrder.Status,

                Items = completeOrder.OrderItems.Select(item => new OrderItemDto//Har OrderItem ko OrderItemDto me convert karo."
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }

        catch
        {
            // कोई भी error → सभी database changes वापस
            await transaction.RollbackAsync();//Agar try ke andar kahin bhi error aaya, yahan aa jao."
            throw; //Rollback kar diya, ab original error ko upar bhej do taaki application ka exception handling system/client ko appropriate error mil sake."
        }//User ke cart ko uthao → check karo cart empty nahi hai → transaction start karo → har product ka latest stock / price verify karo → stock ghatao → OrderItems banao → total calculate karo → Order save karo → cart empty karo → sab successful ho to commit karo, warna sab changes rollback kar do."
    } 
        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId)
    {
        // Logged-in user के सभी orders प्राप्त करो
        var orders = await _orderRepository.GetByUserIdAsync(userId);

        // Orders को clean DTO response में convert करो
        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,

            Items = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        }).ToList();
    }

    public async Task<OrderDto> GetMyOrderByIdAsync(int orderId, int userId)
    {
        // सिर्फ logged-in user का specific order प्राप्त करो
        var order = await _orderRepository
            .GetByIdAndUserIdAsync(orderId, userId);

        // Order नहीं मिला या किसी दूसरे user का है
        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        // Entity को DTO में convert करके return करो
        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,

            Items = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<OrderDto> GetOrderByIdAsync(int orderId)
    {
        // सिर्फ Order ID से order प्राप्त करो
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        // Order Entity को DTO में convert करके return करो
        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,

            Items = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(
    int orderId,
    UpdateOrderStatusDto dto)
    {
        // Order database से प्राप्त करो
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        // अगर वही status दोबारा भेजा गया है
        if (order.Status == dto.Status)
        {
            throw new InvalidOperationException(
                $"Order is already {dto.Status}.");
        }

        // Valid status transition check करो
        var validTransition =
            (order.Status == "Pending" && dto.Status == "Confirmed") ||
            (order.Status == "Confirmed" && dto.Status == "Shipped") ||
            (order.Status == "Shipped" && dto.Status == "Delivered");

        if (!validTransition)
        {
            throw new InvalidOperationException(
                $"Cannot change order status from {order.Status} to {dto.Status}.");
        }

        // Status update करो
        order.Status = dto.Status;

        // Database में save करो
        await _orderRepository.UpdateAsync(order);

        // Updated Order DTO return करो
        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,

            Items = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<OrderDto> CancelOrderAsync( // Logged in user apna order cancel karna chahata hai
    int userId,
    int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);//"Database se order aane tak wait karo, lekin thread ko unnecessarily block mat karo."

        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to cancel this order.");
        }

        if (order.Status == "Cancelled")
        {
            throw new InvalidOperationException(
                "Order is already cancelled.");
        }

        if (order.Status == "Refunded")
        {
            throw new InvalidOperationException(
                "Refunded order cannot be cancelled.");
        }

        if (order.Status != "Pending" &&
            order.Status != "Confirmed")
        {
            throw new InvalidOperationException(
                "Order cannot be cancelled in its current status.");
        }

        if (order.Status == "Confirmed")
        {
            var successfulPayment = order.Payment;

            if (successfulPayment != null &&
                successfulPayment.Status == "Success")
            {
                await _refundService.CreateRefundAsync(
                    userId,
                    successfulPayment.Id);
            }
        }
        order.Status = "Cancelled";

        await _orderRepository.UpdateAsync(order);

        return new OrderDto // Database update ho gaya. Ab controller/client ko updated order ki information deni hai.
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,

            Items = order.OrderItems.Select(item => new OrderItemDto //"Har OrderItem ko OrderItemDto me convert karo."
            {
                ProductId = item.ProductId,
                ProductName = item.Product != null
                    ? item.Product.Name
                    : string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()//Select ke baad result ek LINQ sequence hota hai.
        };
    }

}
