using DataAccess.Data;
using DataAccess.Data.Entities;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsRentTP.Services
{
    public class FavoriteService : IFavService
    {
        private readonly HttpContext httpContext;
        private readonly EquipmentRentalDbContext ctx;

        public FavoriteService(EquipmentRentalDbContext ctx, IHttpContextAccessor contextAccessor)
        {
            this.httpContext = contextAccessor.HttpContext ?? throw new Exception("HttpContext is null.");
            this.ctx = ctx;
        }

        public void Add(int id)
        {
            var existingIds = httpContext.Session.Get<List<int>>("FavItems");
            List<int> ids = existingIds ?? new();

            if (!ids.Contains(id))
            {
                ids.Add(id);
            }

            httpContext.Session.Set("FavItems", ids);
        }

        public void Clear()
        {
            httpContext.Session.Remove("FavItems");
        }

        public int GetFavSize()
        {
            var ids = httpContext.Session.Get<List<int>>("FavItems");
            return ids?.Count ?? 0;
        }

        public List<int> GetItemIds()
        {
            return httpContext.Session.Get<List<int>>("FavItems") ?? new List<int>();
        }

        public List<Equipment> GetEquipments()
        {
            var existingIds = GetItemIds();

            return ctx.Equipments
                .Include(x => x.Category)
                .Where(x => existingIds.Contains(x.Id))
                .ToList();
        }

    }
}

