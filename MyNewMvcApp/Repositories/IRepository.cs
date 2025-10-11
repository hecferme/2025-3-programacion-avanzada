using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgramacionAvanzada.Books.Repositories
{
    public interface IRepository<T>
    {
        Task<T?> GetByPrimaryKeyAsync(object key);
        Task<IEnumerable<T>> GetByNameAsync(string name);
    }
}
