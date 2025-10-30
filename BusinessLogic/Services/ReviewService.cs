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
            var review = await repo.GetByIdAsync(reviewId);
            if (review == null) return;

            var eq = await eqRepo.GetByIdAsync(review.EquipmentId);
            if (eq == null) return;

            eq.reviews.Remove(review);

            eq.ReviewCount--;
            eq.ReviewSum -= review.Rating;
            eq.AverageRating = eq.ReviewCount > 0
                ? (decimal)eq.ReviewSum / eq.ReviewCount
                : 0;

            await repo.DeleteAsync(reviewId);

            await eqRepo.UpdateAsync(eq);
        }

        public async Task EditReview(Review review)
        {
            var oldReview = await repo.GetByIdAsync(review.Id);
            if (oldReview == null) return;

            var eq = await eqRepo.GetByIdAsync(oldReview.EquipmentId);
            if (eq == null) return;

            if (oldReview.Rating != review.Rating)
            {
                eq.ReviewSum = eq.ReviewSum - oldReview.Rating + review.Rating;
                eq.AverageRating = (decimal)eq.ReviewSum / eq.ReviewCount;
                await eqRepo.UpdateAsync(eq);
            }

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

            return repo.GetAllAsync(
                filtering: r => r.EquipmentId == equipmentId)
                       .ContinueWith(t => t.Result.ToList());
        }

        public Task<List<Review>> GetReviewsByUser(string userId)
        {
            var user = userServices.GetById(userId);
            if (user == null) return Task.FromResult(new List<Review>());

            return repo.GetAllAsync(filtering: r => r.UserId == userId)
                       .ContinueWith(t => t.Result.ToList());
        }

        public async Task PostReview(string userId, int equipmentId, string reviewText, int rating)
        {
            var user = await userServices.GetById(userId);
            var equipment = await eqRepo.GetByIdAsync(equipmentId);
            if (user == null || equipment == null) return;

            var review = new Review
            {
                UserId = userId,
                EquipmentId = equipmentId,
                Comment = reviewText,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(review);

            equipment.reviews.Add(review);

            equipment.ReviewCount++;
            equipment.ReviewSum += rating;
            equipment.AverageRating = (decimal)equipment.ReviewSum / equipment.ReviewCount;

            await eqRepo.UpdateAsync(equipment);
        }
    }
}
