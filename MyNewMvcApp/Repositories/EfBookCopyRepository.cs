using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using ProgramacionAvanzada.Books.Hubs;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class EfBookCopyRepository : IBookCopyRepository
    {
        private readonly BooksDbContext _context;
        private readonly IHubContext<BookCopiesStatsHub> _hubContext;

        public EfBookCopyRepository(BooksDbContext context, IHubContext<BookCopiesStatsHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<BookCopy?> GetByPrimaryKeyAsync(object key) => 
            await _context.BookCopies.Include(bc => bc.Book).FirstOrDefaultAsync(bc => bc.Id.Equals(key));

        public async Task<IEnumerable<BookCopy>> GetByNameAsync(string name) => 
            await _context.BookCopies.Include(bc => bc.Book)
                .Where(bc => bc.Serial != null && bc.Serial.ToLower().Contains(name.ToLower()))
                .ToListAsync();

        public async Task<(IEnumerable<BookCopy> Items, int TotalCount)> GetPagedAsync(string name, int pageNumber, int pageSize)
        {
            var query = _context.BookCopies.Include(bc => bc.Book)
                .Where(bc => bc.Serial != null && bc.Serial.ToLower().Contains(name.ToLower()));
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        public async Task<IEnumerable<BookCopy>> GetAllAsync() => 
            await _context.BookCopies.Include(bc => bc.Book).ToListAsync();

        public async Task<IEnumerable<BookCopy>> GetByIsLostAsync(bool isLost) => 
            await _context.BookCopies.Include(bc => bc.Book)
                .Where(bc => bc.IsLost == isLost)
                .ToListAsync();

        public async Task<int> GetTotalCountAsync() => 
            await _context.BookCopies.CountAsync();

        public async Task<int> GetLostCountAsync() => 
            await _context.BookCopies.CountAsync(bc => bc.IsLost == true);

        public async Task<BookCopy> InsertAsync(BookCopy entity)
        {
            _context.BookCopies.Add(entity);
            await _context.SaveChangesAsync();
            await BroadcastStatsUpdate();
            return entity;
        }

        public async Task<BookCopy> UpdateAsync(BookCopy entity)
        {
            _context.BookCopies.Update(entity);
            await _context.SaveChangesAsync();
            await BroadcastStatsUpdate();
            return entity;
        }

        public async Task<bool> DeleteAsync(object key)
        {
            var entity = await _context.BookCopies.FindAsync(key);
            if (entity == null) return false;
            _context.BookCopies.Remove(entity);
            await _context.SaveChangesAsync();
            await BroadcastStatsUpdate();
            return true;
        }

        private async Task BroadcastStatsUpdate()
        {
            var totalCount = await GetTotalCountAsync();
            var lostCount = await GetLostCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveStatsUpdate", totalCount, lostCount);
        }
    }
}
