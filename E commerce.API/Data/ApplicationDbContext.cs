using Microsoft.EntityFrameworkCore;
using E_commerce.API.Models;
namespace E_commerce.API.Data
{
    public class ApplicationDbContext : DbContext // meri Application ko database se baat karni hai, isliye EF Core ka DbContext use karo//
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options) { } // yaha option me datbase ki configuration aati hai//
        public DbSet<Category> Categories { get; set; } // in models ke liye datbase me table manage karni hai..//
        public DbSet<Product> Products { get; set; } // <.Table.> is ke andar ka part Table hota hai...Aur _context.Products Likhkar Product table pe query karsakte hai//

        public DbSet<User> Users { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<WishlistItem> WishlistItems { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Refund> Refunds { get; set; }

        public DbSet<ReturnRequest> ReturnRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)// developrer kahe raha hai,Parent DBContext ka jo OnModelCreating() mathod hai,main uska Apna version Likhraha hu isliye ovveride use kiya ...ModelBuilder EF core ki built in class hai..Ye DB model config karne ka toll milraha hai//
        {                                                            // yahan developer EF core ko rules btata hai ..one to one  & one to many
            base.OnModelCreating(modelBuilder); // pehle parent(base) DbContext ki default model Configuration bhi execute kar do//

            // One User → One Cart
            modelBuilder.Entity<Cart>() // ModelBuilder,mjhe Cart ki Configuration karni hai//
                .HasOne(c => c.User)   // ek cart me kitne User hai ..Cart ke User navigation property ko use karo//
                .WithOne()     // ek User ka kitna Cart Hai//
                .HasForeignKey<Cart>(c => c.UserId)// Cart table me UserId foreign key hai,jo User ko identify karegi//
                .OnDelete(DeleteBehavior.Cascade); // agar user delet hojaye to uska cart bhi delete hojaye//

            // One Cart → Many CartItems  
            modelBuilder.Entity<CartItem>() // ModelBuilder,mjhe CartItem ki Configuration karni hai//
                .HasOne(ci => ci.Cart)      // Ek CartItem kis Cart ka hai? //
                .WithMany(c => c.CartItems)  // ek Cart ka kitna CartItem ho sakte Hai//
                .HasForeignKey(ci => ci.CartId) // CartItem table me CartId foreign key hai,jo CartItem ko identify karegi//
                .OnDelete(DeleteBehavior.Cascade);//Agar Cart delete hua to uska andar ke saare CartItems bhi deletkar do//

            // One Product → Many CartItems
            modelBuilder.Entity<CartItem>() 
                .HasOne(ci => ci.Product) // ek CartItem kis Product ko represent karta hai?//
                .WithMany()               // ek Product multiple CartItem me hosakata hai//
                .HasForeignKey(ci => ci.ProductId) // CartItem table me ProductId foreign key hai
                .OnDelete(DeleteBehavior.Restrict);// agar Product kisi CartItem me use horha hai,to Product ko directly delete matkardena//

            modelBuilder.Entity<Review>() //Ab main Review entity/table ki configuration kar raha hoon."
                .HasOne(r => r.Product) //Ek Review ke saath kitna Product connected hai?"
                .WithMany() //Ek Review ek Product ka hai, lekin ek Product ke kitne Reviews ho sakte hain?"
                .HasForeignKey(r => r.ProductId) //Ek Review ek Product ka hai, lekin ek Product ke kitne Reviews ho sakte hain?"
                .OnDelete(DeleteBehavior.Cascade); // Ek Review ek Product ka hai, lekin ek Product ke kitne Reviews ho sakte hain?"

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User) //Ek Review kis User ka hai
                .WithMany() //Ek User kitne Reviews likh sakta hai?"
                .HasForeignKey(r => r.UserId) //Review table me User ko identify karne wali property UserId hai."
                .OnDelete(DeleteBehavior.Restrict); //Agar User ke reviews hain, to User ko delete karne se pehle mujhe reviews ka issue handle karna padega. Automatically reviews delete mat karna."

            modelBuilder.Entity<Review>() //"Ek user ek product ko baar-baar review na kar sake."
                .HasIndex(r => new { r.UserId, r.ProductId }) // Database me in columns par index/constraint banana hai  ...Yahan anonymous object ban raha hai jisme 2 properties hain:
                .IsUnique();  // ye combination database me duplicate nahi hone chahiye

            modelBuilder.Entity<WishlistItem>()
                .HasOne(w => w.User)//Ek WishlistItem → ek User.
                .WithMany() //Ek User → many WishlistItems.
                .HasForeignKey(w => w.UserId)//UserId relationship ko connect karega.
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);//Product hi delete ho gaya to wishlist me us Product ka reference rakhne ka kya fayda?

            modelBuilder.Entity<WishlistItem>()//"Ek user same product ko wishlist me multiple baar add nahi kar sakta."
                .HasIndex(w => new { w.UserId, w.ProductId })
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order) // ek payment ka ek order
                .WithOne(o => o.Payment) // ek order ka bhi ek paymnet hoga
                .HasForeignKey<Payment>(p => p.OrderId)// payment table ke andar OrderId work as Foreign key 
                .OnDelete(DeleteBehavior.Cascade);// agar kisi oreder ko delete karenge to uska payment bhi delete hojayega

            modelBuilder.Entity<Refund>()
              .HasOne(r => r.Payment) //ek refund ek payment ka hoga
              .WithMany()// ek payment ke multiple refund hosakte hai
              .HasForeignKey(r => r.PaymentId) //
              .OnDelete(DeleteBehavior.Cascade); // agar payment delete hota hai us payment se juda refund automaticaly delete hojayenge

            modelBuilder.Entity<ReturnRequest>()
               .HasOne(r => r.Order)
               .WithMany()
               .HasForeignKey(r => r.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReturnRequest>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);


        }//Ek User ka ek Cart hoga. User delete hua to Cart delete karo. Ek Cart ke bahut saare CartItems ho sakte hain. Cart delete hua to CartItems delete karo. Har CartItem ek Product ko reference karega, lekin Product ko delete karne se pehle check karo ki wo kisi CartItem me use to nahi ho raha.

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Shipment> Shipments { get; set; }

        public DbSet<Coupon> Coupons { get; set; }

        public DbSet<CouponUsage> CouponUsages { get; set; }


    }
}
