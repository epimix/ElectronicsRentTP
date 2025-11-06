using DataAccess.Data;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Interfaces;
namespace ElectronicsRentTP.Services
{
    public class BookingService : IBookService
    {
        private readonly HttpContext httpContext;

        public BookingService(EquipmentRentalDbContext ctx, IHttpContextAccessor contextAccessor)
        {
            this.httpContext = contextAccessor.HttpContext ?? throw new Exception("HttpContext is null.");
        }
        public void Add(int id)
        {
            var existingIds = httpContext.Session.Get<List<int>>("BookItems");
            List<int> ids = existingIds ?? new();

            if (!ids.Contains(id))
            {
                ids.Add(id);
            }

            httpContext.Session.Set("BookItems", ids);
        }
    }

}
