using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class EfAuthorRepository : IAuthorRepository
    {
        private readonly BooksDbContext _context;
        public EfAuthorRepository(BooksDbContext context) => _context = context;

        public async Task<Author?> GetByPrimaryKeyAsync(object key) => await _context.Authors.FindAsync(key);
        public async Task<IEnumerable<Author>> GetByNameAsync(string name) => await _context.Authors.Where(a => a.Name != null && a.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        public async Task<IEnumerable<Author>> GetByCountryAsync(string country) => await _context.Authors.Where(a => a.Country != null && a.Country.ToLower().Contains(country.ToLower())).ToListAsync();

        public async Task<Author> InsertAsync(Author entity)
        {
            _context.Authors.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<Author> UpdateAsync(Author entity)
        {
            _context.Authors.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> DeleteAsync(object key)
        {
            var entity = await _context.Authors.FindAsync(key);
            if (entity == null) return false;
            _context.Authors.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
