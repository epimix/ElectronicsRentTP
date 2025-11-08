using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectronicsRentTP.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService reviewService;
        private readonly IEquipmentService equipmentService;
        private readonly UserManager<User> userManager;

        public ReviewsController(
            IReviewService reviewService,
            IEquipmentService equipmentService,
            UserManager<User> userManager)
        {
            this.reviewService = reviewService;
            this.equipmentService = equipmentService;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int equipmentId, string comment, int rating)
        {
            var userId = userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(comment) || rating < 1 || rating > 5)
            {
                TempData["Error"] = "Please enter a valid comment and rating.";
                return RedirectToAction("Details", "Equipment", new { id = equipmentId });
            }

            await reviewService.PostReview(userId, equipmentId, comment, rating);

            TempData["Success"] = "Your review has been added successfully!";
            return RedirectToAction("Details", "Equipment", new { id = equipmentId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int reviewId, int equipmentId)
        {
            await reviewService.DeleteReview(reviewId);
            TempData["Info"] = "Review deleted successfully.";
            return RedirectToAction("Details", "Equipment", new { id = equipmentId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int reviewId, string comment, int rating, int equipmentId)
        {
            if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "Please provide valid rating and comment.";
                return RedirectToAction("Details", "Equipment", new { id = equipmentId });
            }

            var review = await reviewService.GetReview(reviewId);
            if (review == null)
            {
                TempData["Error"] = "Review not found.";
                return RedirectToAction("Details", "Equipment", new { id = equipmentId });
            }

            var userId = userManager.GetUserId(User);
            if (review.UserId != userId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You can edit only your own reviews.";
                return RedirectToAction("Details", "Equipment", new { id = equipmentId });
            }

            review.Comment = comment;
            review.Rating = rating;
            await reviewService.EditReview(review);

            TempData["Success"] = "Review updated successfully!";
            return RedirectToAction("Details", "Equipment", new { id = equipmentId });
        }

        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            var userId = userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var reviews = await reviewService.GetReviewsByUser(userId);
            return View(reviews);
        }
    }
}
