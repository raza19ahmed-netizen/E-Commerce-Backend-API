namespace E_commerce.API.Models;

public class Order//Jab customer checkout kare, to uski purchase ka ek Order record banao, jisme user kaun hai, order kab hua, total kitna hai, status kya hai aur kaun-kaun se products order hue hain—sab information rahe."
{
    public int Id { get; set; }//"Har Order ki ek unique ID honi chahiye."

    // यह Order किस User ने किया है
    public int UserId { get; set; }//Mujhe record karna hai ki ye Order kis user ne kiya."

    public User User { get; set; } = null!;//Sirf UserId nahi, zarurat padne par poora User object bhi Order ke saath access kar sakun." null! ka matlab "Abhi initialization nahi kar raha, lekin mujhe pata hai runtime par User available hoga

    // Order कब create हुआ
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;//Order kis date/time par bana, ye save karna hai."

    // सभी OrderItems की कुल कीमत
    public decimal TotalAmount { get; set; }//Customer ne poore Order ke liye total kitne paise pay/owe kiye, wo store karna hai."

    // उदाहरण: Pending, Confirmed, Shipped, Delivered
    public string Status { get; set; } = "Pending";//Order ki current condition/status bhi track karni hai


    public Payment Payment { get; set; } = null!;
    // एक Order में कई Products हो सकते हैं
    public ICollection<OrderItem> OrderItems { get; set; }//Ek Order ke andar multiple products ho sakte hain, isliye OrderItems ki collection rakho.
        = new List<OrderItem>(); //"Jab naya Order object banega, uski OrderItems collection initially empty list ho."
}