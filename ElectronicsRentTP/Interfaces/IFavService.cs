using DataAccess.Data.Entities;

namespace ElectronicsRentTP.Interfaces
{
    public interface IFavService
    {
        List<int> GetItemIds();
        List<Equipment> GetEquipments();

        void Add(int id);
        void Clear();
        int GetFavSize();
    }
}

