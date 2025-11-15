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
        public async Task<(IEnumerable<Theme> Items, int TotalCount)> GetPagedAsync(string name, int pageNumber, int pageSize)
        {
            var query = _context.Themes.Where(t => t.ThemeName != null && t.ThemeName.ToLower().Contains(name.ToLower()));
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }
        public async Task<IEnumerable<Theme>> GetBySubjectAsync(string subject) => await _context.Themes.Where(t => t.Subject != null && t.Subject.ToLower().Contains(subject.ToLower())).ToListAsync();

        public async Task<Theme> InsertAsync(Theme entity)
        {
            _context.Themes.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<Theme> UpdateAsync(Theme entity)
        {
            _context.Themes.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> DeleteAsync(object key)
        {
            var entity = await _context.Themes.FindAsync(key);
            if (entity == null) return false;
            _context.Themes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
