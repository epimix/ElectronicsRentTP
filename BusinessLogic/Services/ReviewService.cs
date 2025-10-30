using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> repo;
        private readonly IRepository<Equipment> eqRepo;
        private readonly IUserServices userServices;

        public ReviewService(IRepository<Review> repo, IRepository<Equipment> eqRepo, IUserServices userServices)
        {
            this.repo = repo;
            this.eqRepo = eqRepo;
            this.userServices = userServices;
        }

        public async Task DeleteReview(int reviewId)
        {
            await repo.DeleteAsync(reviewId);
        }

        public async Task EditReview(Review review)
        {
            if (review == null) return;
            await repo.UpdateAsync(review);
        }

        public async Task<Review?> GetReview(int reviewId)
        {
            if (reviewId <= 0) return null;
            return await repo.GetByIdAsync(reviewId);
        }

        public Task<List<Review>> GetReviewsByEquipment(int equipmentId)
        {
            var eq = eqRepo.GetByIdAsync(equipmentId);
            if (eq == null) return Task.FromResult(new List<Review>());

            return repo.GetAllAsync(r => r.EquipmentId == equipmentId)
                       .ContinueWith(t => t.Result.ToList());
        }

        public Task<List<Review>> GetReviewsByUser(string userId)
        {
            var user = userServices.GetById(userId);
            if (user == null) return Task.FromResult(new List<Review>());

            return repo.GetAllAsync(r => r.UserId == userId)
                       .ContinueWith(t => t.Result.ToList());
        }

        public async Task PostReview(string userId, int equipmentId, string reviewText, int rating)
        {
            var user = userServices.GetById(userId);
            var equipment = eqRepo.GetByIdAsync(equipmentId);
            if (user == null || equipment == null) return;

            await repo.AddAsync(new Review
            {
                UserId = userId,
                EquipmentId = equipmentId,
                Comment = reviewText,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
