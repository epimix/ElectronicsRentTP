using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ICategoryService
    {
        Task<IList<EquipmentCategory>> GetAll(
            string? ByName,
            int pageNumber
        );
        Task<IList<EquipmentCategory>> GetAll();
        Task<EquipmentCategory?> GetById(int id);
        Task AddEquipment(EquipmentCategory cat);
        Task UpdateEquipment(EquipmentCategory cat);
        Task DeleteEquipment(int id);
    }
}
