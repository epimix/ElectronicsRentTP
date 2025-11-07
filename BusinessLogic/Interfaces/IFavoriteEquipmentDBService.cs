using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IFavoriteEquipmentDBService
    {
        Task AddToFavorites(string userId, int equipmentId);
        Task<IList<DataAccess.Data.Entities.Equipment>> GetFavoriteEquipmentIds(string userId);
        Task RemoveFromFavorites(string userId, int equipmentId);
    }
}
