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
    }
}
