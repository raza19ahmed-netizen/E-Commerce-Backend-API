namespace E_commerce.API.DTOs  // data transfer object ...DTO ka main kaam data ko ek layer(client) se dusri(Api ke beech) layer tak sefely transfer karna //
{ 
    public class ProductDto   //  Get Api user ko kya dikhana hai ...response//
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; } = string.Empty;// hum respone me category ka pura object nhi bhejna chahte sirf usk name bhejna chate hai
    }
}