using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IEquipmentService
    {
        Task<IList<Equipment>> GetAll(
            int? filterCategoryId,
            string? ByName,
            string? ByDescription,
            decimal? filterMin,
            decimal? filterMax,
            bool? SortPriceAsc,
            bool? IsAvailable,
            int pageNumber
            );
        Task<Equipment?> GetById(int id);
        Task AddEquipment(Equipment equipment);
        Task UpdateEquipment(Equipment equipment);
        Task DeleteEquipment(int id);
    }
}
