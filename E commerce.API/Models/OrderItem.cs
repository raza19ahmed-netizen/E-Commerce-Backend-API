using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.API.Models;

public class OrderItem //Order batata hai poori purchase ke baare mein, aur OrderItem batata hai us purchase ke andar ek particular product ke baare mein
{
    public int Id { get; set; }//Har OrderItem ki apni unique ID.

    // यह Item किस Order का हिस्सा है
    public int OrderId { get; set; }//Ye OrderItem kis Order ka part hai

    public Order Order { get; set; } = null!;//Mujhe sirf OrderId nahi, zarurat padne par associated Order object bhi access karna hai."

    // कौन सा Product खरीदा गया
    public int ProductId { get; set; }//Is OrderItem me kaunsa Product purchase hua

    public Product Product { get; set; } = null!;//Associated Product ka complete object.

    // कितनी Quantity खरीदी गई
    public int Quantity { get; set; }//Customer ne is product ki kitni quantity purchase ki

    // Order करते समय Product की Price
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; } //Order ke time ek product ki price kya thi, wo save karni hai."
}
