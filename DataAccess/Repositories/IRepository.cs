using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IRepository<T> where T : class, BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync(
            int? pageNumber = 1,
            int pageSize = 10,
            Expression<Func<T, bool>>? filtering = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params string[]? includes);
        Task<IList<T>> GetAllAsync();
        Task<int> CountAsync(Expression<Func<T, bool>>? filtering = null);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
        Task ClearAsync();
        Task SaveChange();
    }
}
