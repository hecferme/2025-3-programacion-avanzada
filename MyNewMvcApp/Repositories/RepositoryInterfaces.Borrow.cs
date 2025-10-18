using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;

namespace ProgramacionAvanzada.Books.Repositories
{
    public interface IBorrowRepository : IRepository<Borrow>
    {
        Task<IEnumerable<Borrow>> GetByPersonIdAsync(int personId);
        Task<IEnumerable<Borrow>> GetByBookCopyIdAsync(int bookCopyId);
    }
}
