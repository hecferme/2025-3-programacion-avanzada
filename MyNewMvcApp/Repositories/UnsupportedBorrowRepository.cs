using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;

namespace ProgramacionAvanzada.Books.Repositories
{
    // Used when JSON-only mode is selected but Borrow operations are not supported in JSON fallback.
    public class UnsupportedBorrowRepository : IBorrowRepository
    {
        public Task<Borrow?> GetByPrimaryKeyAsync(object key) => throw new System.NotSupportedException("Borrow repository not available in JSON-only mode.");
        public Task<IEnumerable<Borrow>> GetByNameAsync(string name) => throw new System.NotSupportedException("Borrow repository not available in JSON-only mode.");
        public Task<(IEnumerable<Borrow> Items, int TotalCount)> GetPagedAsync(string name, int pageNumber, int pageSize) => throw new System.NotSupportedException("Borrow repository not available in JSON-only mode.");
        public Task<IEnumerable<Borrow>> GetByPersonIdAsync(int personId) => throw new System.NotSupportedException("Borrow repository not available in JSON-only mode.");
        public Task<IEnumerable<Borrow>> GetByBookCopyIdAsync(int bookCopyId) => throw new System.NotSupportedException("Borrow repository not available in JSON-only mode.");
        public Task<Borrow> InsertAsync(Borrow entity) => throw new System.NotSupportedException("Borrow repository not available in JSON-only mode.");
        public Task<Borrow> UpdateAsync(Borrow entity) => throw new System.NotSupportedException("Borrow repository not available in JSON-only mode.");
        public Task<bool> DeleteAsync(object key) => throw new System.NotSupportedException("Borrow repository not available in JSON-only mode.");
    }
}
