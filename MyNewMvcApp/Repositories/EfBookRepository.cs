using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class EfBookRepository : IBookRepository
    {
        private readonly BooksDbContext _context;
        public EfBookRepository(BooksDbContext context) => _context = context;

        public async Task<Book?> GetByPrimaryKeyAsync(object key) => await _context.Books.FindAsync(key);
        public async Task<IEnumerable<Book>> GetByNameAsync(string name) => await _context.Books.Where(b => (b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(name.ToLower())) || (b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(name.ToLower()))).ToListAsync();
        public async Task<IEnumerable<Book>> GetByOriginalTitleAsync(string originalTitle) => await _context.Books.Where(b => b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(originalTitle.ToLower())).ToListAsync();
        public async Task<IEnumerable<Book>> GetByEnglishTitleAsync(string englishTitle) => await _context.Books.Where(b => b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(englishTitle.ToLower())).ToListAsync();

        public async Task<Book> InsertAsync(Book entity)
        {
            _context.Books.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<Book> UpdateAsync(Book entity)
        {
            _context.Books.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> DeleteAsync(object key)
        {
            var entity = await _context.Books.FindAsync(key);
            if (entity == null) return false;
            _context.Books.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
