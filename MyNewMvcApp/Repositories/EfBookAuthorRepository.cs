using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class EfBookAuthorRepository : IBookAuthorRepository
    {
        private readonly BooksDbContext _context;
        public EfBookAuthorRepository(BooksDbContext context) => _context = context;

        public async Task<BookAuthor?> GetByPrimaryKeyAsync(object key)
        {
            if (key is ValueTuple<int, int> tuple)
                return await _context.Set<BookAuthor>()
                    .Include(ba => ba.Book)
                    .Include(ba => ba.Author)
                    .FirstOrDefaultAsync(ba => ba.BookId == tuple.Item1 && ba.AuthorId == tuple.Item2);
            return null;
        }
        public async Task<IEnumerable<BookAuthor>> GetByNameAsync(string name) => await GetByBookNameAsync(name);
        public async Task<IEnumerable<BookAuthor>> GetByBookNameAsync(string bookName)
        {
            return await _context.Set<BookAuthor>()
                .Include(ba => ba.Book)
                .Include(ba => ba.Author)
                .Where(ba => (ba.Book.OriginalTitle != null && ba.Book.OriginalTitle.ToLower().Contains(bookName.ToLower())) || (ba.Book.EnglishTitle != null && ba.Book.EnglishTitle.ToLower().Contains(bookName.ToLower())))
                .ToListAsync();
        }
        public async Task<IEnumerable<BookAuthor>> GetByAuthorNameAsync(string authorName)
        {
            return await _context.Set<BookAuthor>()
                .Include(ba => ba.Book)
                .Include(ba => ba.Author)
                .Where(ba => ba.Author.Name != null && ba.Author.Name.ToLower().Contains(authorName.ToLower()))
                .ToListAsync();
        }

        public async Task<BookAuthor> InsertAsync(BookAuthor entity)
        {
            _context.Set<BookAuthor>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<BookAuthor> UpdateAsync(BookAuthor entity)
        {
            _context.Set<BookAuthor>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> DeleteAsync(object key)
        {
            if (key is ValueTuple<int, int> tuple)
            {
                var entity = await _context.Set<BookAuthor>().FindAsync(tuple.Item1, tuple.Item2);
                if (entity == null) return false;
                _context.Set<BookAuthor>().Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
