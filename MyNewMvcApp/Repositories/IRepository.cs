using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgramacionAvanzada.Books.Repositories
{
    public interface IRepository<T>
    {
        Task<T?> GetByPrimaryKeyAsync(object key);
        Task<IEnumerable<T>> GetByNameAsync(string name);
        Task<T> InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(object key);
    }
}
