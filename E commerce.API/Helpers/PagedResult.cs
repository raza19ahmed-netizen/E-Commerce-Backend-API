namespace E_commerce.API.Helpers
{
    public class PagedResult<T> // EK Reusable Class hai..agar hum Product/Orders/Customers/Catogories sb me Pagination lag jayega//
    {
        public IEnumerable<T> Items { get; set; } = new List<T>(); // current page me jo bhi type ka data arha hai uska collection//

        public int PageNumber { get; set; }  // kaunsa page nikalana hai 1,2,3,4...//

        public int PageSize { get; set; }   // ek page me kitne count ya product hai //

        public int TotalItems { get; set; } // hmare pass kitne product count hai //

        public int TotalPages { get; set; } //hmare pass kitne pages hai jisme total product rakhe hai//
    }
}
