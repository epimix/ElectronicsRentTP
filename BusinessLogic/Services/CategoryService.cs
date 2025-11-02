using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using DataAccess.Repositories;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<EquipmentCategory> repo;

        public CategoryService(IRepository<EquipmentCategory> repo)
        {
            this.repo = repo;
        }
        public async Task AddEquipment(EquipmentCategory cat)
        {
            if(cat == null)
                return;

            await repo.AddAsync(cat);
        }

        public async Task DeleteEquipment(int id)
        {
            var cat = await repo.GetByIdAsync(id);
            if (cat == null)
                return;

            await repo.DeleteAsync(cat);
        }


        public async Task<IList<EquipmentCategory>> GetAll(string? ByName, int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;
            var filterEx = PredicateBuilder.New<EquipmentCategory>(true);

            if (!string.IsNullOrWhiteSpace(ByName))
                filterEx = filterEx.And(x => x.Name.ToLower().Contains(ByName.ToLower()));

            var items = await repo.GetAllAsync(
                pageNumber: pageNumber,
                pageSize: 10,
                filtering: filterEx,
                orderBy: q => q.OrderBy(e => e.Name)
            );

            return items.ToList();
        }

        public async Task<EquipmentCategory?> GetById(int id)
        {
            return await repo.GetByIdAsync(id);

        }

        public async Task UpdateEquipment(EquipmentCategory cat)
        {
            if (cat == null)
                return;

            await repo.UpdateAsync(cat);
        }
    }
}
