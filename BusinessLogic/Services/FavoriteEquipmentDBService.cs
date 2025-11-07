using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class FavoriteEquipmentDBService : IFavoriteEquipmentDBService
    {
        readonly private IRepository<Equipment> eqrepo;
        readonly private IUserServices userServices;
        readonly private EquipmentRentalDbContext ctx;

        public FavoriteEquipmentDBService(
            IRepository<Equipment> eqrepo,
            IUserServices userServices,
            EquipmentRentalDbContext ctx)
        {
            this.eqrepo = eqrepo;
            this.userServices = userServices;
            this.ctx = ctx;
        }

        public async Task AddToFavorites(string userId, int equipmentId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID is invalid", nameof(userId));
            }

            var user = await userServices.GetById(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }

            var equipment = await eqrepo.GetByIdAsync(equipmentId);
            if (equipment == null)
            {
                throw new ArgumentException("Equipment not found", nameof(equipmentId));
            }

            if (user.FavoriteEquipment.Any(e => e.Id == equipmentId))
            {
                throw new InvalidOperationException("This equipment is already in the user's favorites.");
            }

            user.FavoriteEquipment.Add(equipment);
            await ctx.SaveChangesAsync();
        }


        public async Task<IList<Equipment>> GetFavoriteEquipmentIds(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID is invalid", nameof(userId));
            }
            var user = await userServices.GetById(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }

            return user.FavoriteEquipment.ToList();
        }

        public async Task RemoveFromFavorites(string userId, int equipmentId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID is invalid", nameof(userId));
            }

            var user = await userServices.GetById(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }

            var equipment = await eqrepo.GetByIdAsync(equipmentId);
            if (equipment == null)
            {
                throw new ArgumentException("Equipment not found", nameof(equipmentId));
            }

            if (!user.FavoriteEquipment.Any(e => e.Id == equipmentId))
            {
                throw new InvalidOperationException("This equipment is not in the user's favorites.");
            }

            user.FavoriteEquipment.Remove(equipment);
            await ctx.SaveChangesAsync();
        }
    }
}
