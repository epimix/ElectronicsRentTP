using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ICartService
    {
        Task AddToCart(string userId, int equipmentId, int quantity);

        Task<IList<CartEntity>> GetCartItems(string userId);
        Task RemoveFromCart(string userId, int equipmentId);
    }
}
