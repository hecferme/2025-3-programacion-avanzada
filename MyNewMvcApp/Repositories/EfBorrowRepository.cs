using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class EfBorrowRepository : IBorrowRepository
    {
        private readonly BooksDbContext _context;
        public EfBorrowRepository(BooksDbContext context) => _context = context;

        public async Task<Borrow?> GetByPrimaryKeyAsync(object key) => await _context.Borrows.FindAsync(key);
        public async Task<IEnumerable<Borrow>> GetByNameAsync(string name) => await _context.Borrows.Include(b => b.Person).Include(b => b.BookCopy).Where(b => b.Person.Name != null && b.Person.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        public async Task<IEnumerable<Borrow>> GetByPersonIdAsync(int personId) => await _context.Borrows.Where(b => b.PersonId == personId).ToListAsync();
        public async Task<IEnumerable<Borrow>> GetByBookCopyIdAsync(int bookCopyId) => await _context.Borrows.Where(b => b.BookCopyId == bookCopyId).ToListAsync();

        public async Task<Borrow> InsertAsync(Borrow entity)
        {
            _context.Borrows.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Borrow> UpdateAsync(Borrow entity)
        {
            _context.Borrows.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(object key)
        {
            var entity = await _context.Borrows.FindAsync(key);
            if (entity == null) return false;
            _context.Borrows.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
