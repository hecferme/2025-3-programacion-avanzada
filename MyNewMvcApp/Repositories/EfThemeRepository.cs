using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class EfThemeRepository : IThemeRepository
    {
        private readonly BooksDbContext _context;
        public EfThemeRepository(BooksDbContext context) => _context = context;

        public async Task<Theme?> GetByPrimaryKeyAsync(object key) => await _context.Themes.FindAsync(key);
        public async Task<IEnumerable<Theme>> GetByNameAsync(string name) => await _context.Themes.Where(t => t.ThemeName != null && t.ThemeName.ToLower().Contains(name.ToLower())).ToListAsync();
        public async Task<IEnumerable<Theme>> GetBySubjectAsync(string subject) => await _context.Themes.Where(t => t.Subject != null && t.Subject.ToLower().Contains(subject.ToLower())).ToListAsync();
    }
}
