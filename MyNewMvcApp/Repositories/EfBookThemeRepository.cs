using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class EfBookThemeRepository : IBookThemeRepository
    {
        private readonly BooksDbContext _context;
        public EfBookThemeRepository(BooksDbContext context) => _context = context;

        public async Task<BookTheme?> GetByPrimaryKeyAsync(object key)
        {
            if (key is ValueTuple<int, int> tuple)
                return await _context.Set<BookTheme>().FindAsync(tuple.Item1, tuple.Item2);
            return null;
        }
        public async Task<IEnumerable<BookTheme>> GetByNameAsync(string name) => await GetByBookNameAsync(name);
        public async Task<IEnumerable<BookTheme>> GetByBookNameAsync(string bookName) => await _context.Set<BookTheme>().Include(bt => bt.Book).Where(bt => (bt.Book.OriginalTitle != null && bt.Book.OriginalTitle.ToLower().Contains(bookName.ToLower())) || (bt.Book.EnglishTitle != null && bt.Book.EnglishTitle.ToLower().Contains(bookName.ToLower()))).ToListAsync();
        public async Task<IEnumerable<BookTheme>> GetByThemeNameAsync(string themeName) => await _context.Set<BookTheme>().Include(bt => bt.Theme).Where(bt => bt.Theme.ThemeName != null && bt.Theme.ThemeName.ToLower().Contains(themeName.ToLower())).ToListAsync();
        public async Task<IEnumerable<BookTheme>> GetBySubjectNameAsync(string subjectName) => await _context.Set<BookTheme>().Include(bt => bt.Theme).Where(bt => bt.Theme.Subject != null && bt.Theme.Subject.ToLower().Contains(subjectName.ToLower())).ToListAsync();
    }
}
