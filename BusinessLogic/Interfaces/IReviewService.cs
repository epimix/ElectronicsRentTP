using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IReviewService
    {
        Task PostReview(string userId, int equipmentId, string reviewText, int rating);
        Task EditReview(Review review);
        Task DeleteReview(int reviewId);
        Task<List<Review>> GetReviewsByEquipment(int equipmentId);
        Task<List<Review>> GetReviewsByUser(string userId);
        Task<Review?> GetReview(int reviewId);
    }
}
