using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class CartService : ICartService 
{
    private readonly ICartRepository _cartRepository;//Cart se related database ka kaam mujhe khud nahi karna. CartRepository ko karne
    private readonly IProductRepository _productRepository; //Product database me exist karta hai ya nahi, ye check karne ke liye ProductRepository use karunga."

    public CartService(                //CartService ko kaam karne ke liye CartRepository aur ProductRepository chahiye."
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task AddToCartAsync(int userId, AddToCartDto dto)//User userId ke cart me, client ke diye hue dto ke according product add karo."
    {
        // Step 1: Check whether the product exists
        var product = await _productRepository.GetByIdAsync(dto.ProductId);//Client jis ProductId ko bhej raha hai, kya wo Product database me actually exist karta hai?"

        if (product == null) //Agar product database me mila hi nahi, to cart me kaise add karunga?"
        {
            throw new KeyNotFoundException("Product not found.");
        }

        // Step 2: Find the user's cart
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);//"Product valid hai. Ab check karo ki is User ka Cart already bana hua hai ya nahi."

        // Step 3: Create a cart if the user doesn't have one
        if (cart == null) //User ka Cart abhi bana hi nahi hai
        {
            cart = new Cart // ab memory me ek naya Cart Object bnao
            {
                UserId = userId //Naye Cart ke UserId property me current user ki ID daal do.
            };

            cart = await _cartRepository.AddCartAsync(cart); //save hone ke baad database Cart Ko Id dega islye returned Cart Ko dobara  cart me store kiya ab card.Id available hai
        }

        // Step 4: Check whether the product is already in the cart
        var existingCartItem = await _cartRepository.GetCartItemAsync(//Ab check karo ki ye Product is Cart me pehle se pada hua hai ya nahi."
            cart.Id,
            dto.ProductId);

        if (existingCartItem != null)
        {

            var totalQuantity = existingCartItem.Quantity + dto.Quantity;

            if(totalQuantity > product.Stock)
            {
                throw new InvalidOperationException($"only {product.Stock} items are available in stock.");
            }
            existingCartItem.Quantity = totalQuantity; //Customer ne same product dobara add kiya hai, isliye duplicate CartItem mat banao; existing quantity increase karo."

            await _cartRepository.UpdateCartItemAsync(existingCartItem);//Ab updated quantity database me save hogi.
        }
        else
        {
            if (dto.Quantity > product.Stock)
            {
                throw new InvalidOperationException($"only {product.Stock} items are available in stock.");
            }
            // Product doesn't exist → add a new cart item
            var cartItem = new CartItem //Product cart me pehle se nahi hai, isliye naya CartItem banana padega."
            {
                CartId = cart.Id,            // kis Cart Ka? 
                ProductId = dto.ProductId,   //  kis Product ka?
                Quantity = dto.Quantity      //   ktni quantity?
            };

            await _cartRepository.AddCartItemAsync(cartItem); // ye naya CartItem datBase me save kar do //
        
        
       }

    }//User product ko cart me add karna chahta hai. Pehle product valid hai ya nahi check karo → user ka cart dhoondo → cart nahi hai to banao → product pehle se cart me hai to quantity badhao → nahi hai to naya CartItem banao."

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await _cartRepository.GetCartWithItemsAsync(userId);

        if (cart == null)
        {
            return new CartDto(); // agar vart nhi milraha to developer error nahi bhej raha balki empty CarDto return kar rha hai
        }

        return new CartDto //Database ka Cart directly client ko nahi bhejna. Uske CartItems ko CartItemDto me convert karke CartDto banana hai."
        {
            Items = cart.CartItems.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Price = item.Product.Price,
                Quantity = item.Quantity
            }).ToList()  // Select() se converted items mil rahe hain.. ToList() unhe actual List<CartItemDto> me convert Karta Hai kyuki CartDto.Items ka type hai
        };
    }

    public async Task RemoveFromCartAsync(int userId, int productId)
    {
        // Step 1: User की Cart ढूँढो
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            throw new KeyNotFoundException("Cart not found.");
        }

        // Step 2: उस Cart में Product ढूँढो
        var cartItem = await _cartRepository.GetCartItemAsync(
            cart.Id,
            productId);

        if (cartItem == null)
        {
            throw new KeyNotFoundException(
                "Product not found in cart.");
        }

        // Step 3: केवल वह specific CartItem remove करो
        await _cartRepository.RemoveCartItemAsync(cartItem);
    }

    public async Task UpdateCartItemAsync(  //"Logged-in user apne cart me kisi product ki quantity change kar raha hai. Pehle verify karo ki cart hai, phir verify karo ki product usi cart me hai, phir stock check karo, aur sab sahi ho to quantity update karo."
    int userId,
    int productId,
    UpdateCartItemDto dto)
    {
        // Step 1: User की Cart ढूँढो
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            throw new KeyNotFoundException("Cart not found.");
        }

        // Step 2: Cart में Product ढूँढो
        var cartItem = await _cartRepository.GetCartItemAsync(
            cart.Id,
            productId);

        if (cartItem == null)
        {
            throw new KeyNotFoundException("Product not found in cart.");
        }

        // Step 3: Product की current stock जानकारी लो
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        // Step 4: नई Quantity stock से ज्यादा तो नहीं?
        if (dto.Quantity > product.Stock)
        {
            throw new InvalidOperationException(
                $"Only {product.Stock} items are available in stock.");
        }

        // Step 5: नई quantity set करो
        cartItem.Quantity = dto.Quantity;

        await _cartRepository.UpdateCartItemAsync(cartItem);
    }

    public async Task ClearCartAsync(int userId)
    {
        // Step 1: Logged-in user की Cart ढूँढो
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            throw new KeyNotFoundException("Cart not found.");
        }

        // Step 2: Cart के सभी items हटाओ
        await _cartRepository.ClearCartAsync(cart.Id);
    }
}