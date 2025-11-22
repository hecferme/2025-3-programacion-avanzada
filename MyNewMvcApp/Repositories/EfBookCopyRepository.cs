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
            // Track the original entity to check if IsLost status changed
            var originalEntity = await _context.BookCopies.AsNoTracking()
                .FirstOrDefaultAsync(bc => bc.Id == entity.Id);

            // Set LostDate when IsLost changes from false to true
            if (originalEntity != null && originalEntity.IsLost != true && entity.IsLost == true)
            {
                entity.LostDate = DateTime.Now;
            }
            // Clear LostDate when IsLost changes from true to false
            else if (originalEntity != null && originalEntity.IsLost == true && entity.IsLost != true)
            {
                entity.LostDate = null;
            }

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

        public async Task<int> GetLostBooksByHourForTodayAsync(int hour)
        {
            var today = DateTime.Today;
            var startOfHour = today.AddHours(hour);
            var endOfHour = startOfHour.AddHours(1);

            return await _context.BookCopies
                .CountAsync(bc => bc.LostDate != null && 
                                  bc.LostDate >= startOfHour && 
                                  bc.LostDate < endOfHour);
        }

        public async Task<int[]> GetLostBooksHourlyDataForTodayAsync()
        {
            var today = DateTime.Today;
            var hourlyCounts = new int[24];

            // Get all books lost today
            var lostBooksToday = await _context.BookCopies
                .Where(bc => bc.LostDate != null && 
                             bc.LostDate >= today && 
                             bc.LostDate < today.AddDays(1))
                .Select(bc => bc.LostDate!.Value)
                .ToListAsync();

            // Count books per hour
            foreach (var lostDate in lostBooksToday)
            {
                hourlyCounts[lostDate.Hour]++;
            }

            return hourlyCounts;
        }

        private async Task BroadcastStatsUpdate()
        {
            var totalCount = await GetTotalCountAsync();
            var lostCount = await GetLostCountAsync();
            var hourlyData = await GetLostBooksHourlyDataForTodayAsync();
            
            await _hubContext.Clients.All.SendAsync("ReceiveStatsUpdate", totalCount, lostCount);
            await _hubContext.Clients.All.SendAsync("ReceiveHourlyChartUpdate", hourlyData);
        }
    }
}
