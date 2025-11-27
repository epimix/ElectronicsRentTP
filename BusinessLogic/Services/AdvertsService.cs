using AutoMapper;
using BusinessLogic.Dtos;
using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AdvertsService : IAdvertsService
    {
        IUserServices userService;
        private readonly IMapper _mapper;
        private readonly EquipmentRentalDbContext ctx;

        public AdvertsService(IUserServices userService, EquipmentRentalDbContext ctx, IMapper _mapper)
        {
            this.userService = userService;
            this.ctx = ctx;
            this._mapper = _mapper;
        }
        public async Task<Equipment?> GetById(int id)
        {
            return await ctx.Equipments
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<IList<Equipment>> GetCartItems(string userId)
        {
            var user = await userService.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            return user.MyAdverts;
        }
        public async Task UpdateAdvert(EquipmentDTO dto)
        {
            var existing = await ctx.Equipments.FindAsync(dto.Id);

            if (existing == null)
                throw new Exception("Equipment not found");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.CategoryId = dto.CategoryId;
            existing.PricePerHour = dto.PricePerHour;
            existing.Quantity = dto.Quantity;
            existing.ImageUrl = dto.ImageUrl;

            await ctx.SaveChangesAsync();
        }

        public async Task CreateAdvert(EquipmentDTO dto, string userId)
        {
            Equipment equipment = _mapper.Map<Equipment>(dto);
            equipment.OwnerId = userId;

            ctx.Equipments.Add(equipment);
            await ctx.SaveChangesAsync();
        }

        public bool CanEdit(Equipment equipment, string userId, bool isAdmin)
        {
            var isFromDatabase = string.IsNullOrEmpty(equipment.OwnerId) ||
                                 equipment.OwnerId == "551b73c1-3601-490c-90bf-5af17a4408d5";

            return isAdmin || (!isFromDatabase && equipment.OwnerId == userId);
        }

        public bool CanDelete(Equipment equipment, string userId, bool isAdmin)
        {
            return isAdmin || equipment.OwnerId == userId;
        }
        public async Task DeleteAdvert(int id)
        {
            var equipment = await ctx.Equipments.FindAsync(id);

            if (equipment == null)
                throw new Exception("Equipment not found");

            ctx.Equipments.Remove(equipment);
            await ctx.SaveChangesAsync();
        }
    }
}
