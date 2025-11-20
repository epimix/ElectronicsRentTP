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
    public class CartService : ICartService
    {
        private readonly IRepository<Equipment> equipmentRepo;
        private readonly IUserServices userService;
        private readonly IRepository<CartEntity> repo;

        public CartService(IRepository<Equipment> equipmentRepo, IUserServices userService, IRepository<CartEntity> repo)
        {
            this.equipmentRepo = equipmentRepo;
            this.userService = userService;
            this.repo = repo;
        }
        public async Task AddToCart(string userId, int equipmentId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            if (equipmentId <= 0)
                throw new ArgumentException("Invalid equipment ID.");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid user ID.");

            var equipment = await equipmentRepo.GetByIdAsync(equipmentId);
            if (equipment == null)
                throw new ArgumentException("Equipment not found.");

            var user = await userService.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            if (user.Carts.Any(c => c.EquipmentId == equipmentId))
                throw new InvalidOperationException("Equipment is already in the cart.");

            var cartItem = new CartEntity
            {
                UserId = userId,
                EquipmentId = equipmentId,
                Quantity = quantity,
                TotalPrice = equipment.PricePerHour * quantity
            };

            await userService.AddToCart(cartItem);

            equipment.Quantity = equipment.Quantity - quantity;
            await repo.SaveChange();
        }

        public async Task<IList<CartEntity>> GetCartItems(string userId)
        {
            var user = await userService.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            // Завантажуємо кожен товар повністю з БД
            foreach (var cartItem in user.Carts)
            {
                cartItem.Equipment = await equipmentRepo.GetByIdAsync(cartItem.EquipmentId);
            }

            return user.Carts;
        }

        public async Task RemoveFromCart(string userId, int equipmentId)
        {
            var user = await userService.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found.");
            var cartItem = user.Carts.FirstOrDefault(c => c.EquipmentId == equipmentId);
            if (cartItem == null)
                throw new ArgumentException("Equipment not found in cart.");
            var eq = await equipmentRepo.GetByIdAsync(equipmentId);
            if (eq == null)
                throw new ArgumentException("Equipment not found.");
            user.Carts.Remove(cartItem);

            eq.Quantity = eq.Quantity + cartItem.Quantity;
            await repo.SaveChange();
        }
    }
}
