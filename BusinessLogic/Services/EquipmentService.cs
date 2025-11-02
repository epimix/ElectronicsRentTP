using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using DataAccess.Repositories;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IRepository<Equipment> repo;

        public EquipmentService(IRepository<Equipment> repo)
        {
            this.repo = repo;
        }

        public async Task<IList<Equipment>> GetAll(
            int? filterCategoryId,
            string? ByName,
            string? ByDescription,
            decimal? filterMin,
            decimal? filterMax,
            bool? SortPriceAsc,
            bool? IsAvailable,
            int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            var filterEx = PredicateBuilder.New<Equipment>(true);

            if (filterCategoryId != null)
                filterEx = filterEx.And(x => x.CategoryId == filterCategoryId);

            if (!string.IsNullOrWhiteSpace(ByName))
                filterEx = filterEx.And(x => x.Name.ToLower().Contains(ByName.ToLower()));

            if (!string.IsNullOrWhiteSpace(ByDescription))
                filterEx = filterEx.And(x => x.Description.ToLower().Contains(ByDescription.ToLower()));

            if (filterMin != null)
                filterEx = filterEx.And(x => x.PricePerHour >= filterMin.Value);

            if (filterMax != null)
                filterEx = filterEx.And(x => x.PricePerHour <= filterMax.Value);

            if (IsAvailable != null)
                filterEx = filterEx.And(x => x.IsAvailable == IsAvailable.Value);

            var items = await repo.GetAllAsync(
                pageNumber: 1,
                pageSize: 10,
                filtering: filterEx,
                orderBy: q =>
                    SortPriceAsc == true ? q.OrderBy(e => e.PricePerHour) :
                    SortPriceAsc == false ? q.OrderByDescending(e => e.PricePerHour) :
                    q.OrderBy(e => e.Id),
                nameof(Equipment.Category), nameof(Equipment.reviews)
            );
            return items.ToList();
        }


        public async Task AddEquipment(Equipment equipment)
        {
            if (equipment == null)
                return;

            await repo.AddAsync(equipment);
        }

        public async Task DeleteEquipment(int id)
        {
            var equipment = await repo.GetByIdAsync(id);
            if (equipment == null)
                return;

            await repo.DeleteAsync(equipment);
        }

        public async Task<Equipment?> GetById(int id)
        {
            return await repo.GetByIdAsync(id);
        }

        public async Task UpdateEquipment(Equipment equipment)
        {
            if (equipment == null)
                return;

            await repo.UpdateAsync(equipment);
        }
    }
}
