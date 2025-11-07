namespace ElectronicsRentTP.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }
}

