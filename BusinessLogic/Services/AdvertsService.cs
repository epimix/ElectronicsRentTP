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
        private readonly EquipmentRentalDbContext ctx;

        public AdvertsService(IUserServices userService, EquipmentRentalDbContext ctx)
        {
            this.userService = userService;
            this.ctx = ctx;
        }
        public async Task<IList<Equipment>> GetCartItems(string userId)
        {
            var user = await userService.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            return user.MyAdverts;
        }
    }
}
