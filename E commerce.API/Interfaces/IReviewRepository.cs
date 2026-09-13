using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IReviewRepository
{
    Task<Review> AddAsync(Review review);//User naya review submit karega. Mujhe Review ko database me insert karna padega

    Task<bool> ExistsAsync(int userId, int productId);//Kya is user ne is product ko already review kar diya hai?"

    Task<IEnumerable<Review>> GetByProductIdAsync(int productId);//Customer jab Product #25 dekhega, us product ke saare reviews dikhane hain."

    Task<(double AverageRating, int TotalReviews)> GetRatingSummaryAsync(int productId);//"Product page par sirf reviews ki list nahi, mujhe rating summary bhi dikhani hai."

    Task<Review?> GetByIdAndUserIdAsync(int reviewId, int userId);//"Kya Review #101 actually User #8 ka hai?"...Review ki ID pata hona enough nahi hai; review kis user ka hai, ye bhi verify karo."

    Task UpdateAsync(Review review);

    Task DeleteAsync(Review review);

}
