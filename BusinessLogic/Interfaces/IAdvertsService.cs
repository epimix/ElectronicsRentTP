using BusinessLogic.Dtos;
using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAdvertsService
    {
        Task<IList<Equipment>> GetCartItems(string userId);
        Task CreateAdvert(EquipmentDTO dto, string userId);
        bool CanEdit(Equipment equipment, string userId, bool isAdmin);
        bool CanDelete(Equipment equipment, string userId, bool isAdmin);
        Task UpdateAdvert(EquipmentDTO dto);
        Task DeleteAdvert(int id);
        Task<Equipment?> GetById(int id);
    }
}
