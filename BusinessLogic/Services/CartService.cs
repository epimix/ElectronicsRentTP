using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using DataAccess.Repositories;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
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
        private readonly EquipmentRentalDbContext ctx;

        public CartService(IRepository<Equipment> equipmentRepo, IUserServices userService, IRepository<CartEntity> repo, EquipmentRentalDbContext ctx)
        {
            this.equipmentRepo = equipmentRepo;
            this.userService = userService;
            this.repo = repo;
            this.ctx = ctx;
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

            if (equipment.Quantity < quantity)
                throw new InvalidOperationException($"Not enough quantity available. Only {equipment.Quantity} items in stock.");

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
            await equipmentRepo.UpdateAsync(equipment);
        }

        public async Task<IList<CartEntity>> GetCartItems(string userId)
        {
            var user = await userService.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            // Завантажуємо кожен товар повністю з БД
            foreach (var cartItem in user.Carts)
            {
                var equipment = await equipmentRepo.GetByIdAsync(cartItem.EquipmentId);
                if (equipment != null)
                {
                    cartItem.Equipment = equipment;
                }
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
            
            // Повертаємо товар на склад
            eq.Quantity = eq.Quantity + cartItem.Quantity;
            await equipmentRepo.UpdateAsync(eq);

            // Видаляємо з кошика
            user.Carts.Remove(cartItem);
            await userService.Update(user);
        }

        public async Task UpdateQuantity(string userId, int equipmentId, int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var user = await userService.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            var cartItem = user.Carts.FirstOrDefault(c => c.EquipmentId == equipmentId);
            if (cartItem == null)
                throw new ArgumentException("Equipment not found in cart.");

            var equipment = await equipmentRepo.GetByIdAsync(equipmentId);
            if (equipment == null)
                throw new ArgumentException("Equipment not found.");

            var quantityDifference = newQuantity - cartItem.Quantity;

            if (quantityDifference > 0)
            {
                // Збільшуємо кількість - перевіряємо наявність
                if (equipment.Quantity < quantityDifference)
                    throw new InvalidOperationException($"Not enough quantity available. Only {equipment.Quantity} items in stock.");
            }

            // Використовуємо SQL для оновлення без Entity Framework Update
            var newTotalPrice = equipment.PricePerHour * newQuantity;
            await ctx.Database.ExecuteSqlRawAsync(
                "UPDATE CartEntities SET Quantity = {0}, TotalPrice = {1} WHERE UserId = {2} AND EquipmentId = {3}",
                newQuantity, newTotalPrice, userId, equipmentId);
            
            // Оновлюємо кількість товару через SQL (віднімаємо різницю)
            if (quantityDifference != 0)
            {
                await ctx.Database.ExecuteSqlRawAsync(
                    "UPDATE Equipments SET Quantity = Quantity - {0} WHERE Id = {1}",
                    quantityDifference, equipmentId);
            }
        }
    }
}
