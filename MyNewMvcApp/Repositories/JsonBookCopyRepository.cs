using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using ProgramacionAvanzada.Books.Hubs;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class JsonBookCopyRepository : IBookCopyRepository
    {
        private readonly string _jsonFilePath;
        private readonly IHubContext<BookCopiesStatsHub>? _hubContext;

        public JsonBookCopyRepository(string filePath, IHubContext<BookCopiesStatsHub>? hubContext = null)
        {
            _jsonFilePath = filePath;
            _hubContext = hubContext;
        }

        private async Task<List<BookCopy>> LoadAsync()
        {
            if (!File.Exists(_jsonFilePath)) return new List<BookCopy>();
            return JsonSerializer.Deserialize<List<BookCopy>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new List<BookCopy>();
        }

        private async Task SaveAsync(IEnumerable<BookCopy> list)
        {
            var dir = Path.GetDirectoryName(_jsonFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
        }

        public async Task<BookCopy?> GetByPrimaryKeyAsync(object key)
        {
            var list = await LoadAsync();
            return list.FirstOrDefault(bc => bc.Id.Equals(key));
        }

        public async Task<IEnumerable<BookCopy>> GetByNameAsync(string name)
        {
            var list = await LoadAsync();
            if (string.IsNullOrWhiteSpace(name)) return list;
            return list.Where(bc => bc.Serial != null && bc.Serial.ToLower().Contains(name.ToLower()));
        }

        public async Task<(IEnumerable<BookCopy> Items, int TotalCount)> GetPagedAsync(string name, int pageNumber, int pageSize)
        {
            var filtered = (await GetByNameAsync(name)).ToList();
            var total = filtered.Count;
            var items = filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (items, total);
        }

        public async Task<IEnumerable<BookCopy>> GetAllAsync()
        {
            return await LoadAsync();
        }

        public async Task<IEnumerable<BookCopy>> GetByIsLostAsync(bool isLost)
        {
            var list = await LoadAsync();
            return list.Where(bc => bc.IsLost == isLost);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return (await LoadAsync()).Count;
        }

        public async Task<int> GetLostCountAsync()
        {
            return (await LoadAsync()).Count(bc => bc.IsLost == true);
        }

        public async Task<BookCopy> InsertAsync(BookCopy entity)
        {
            var list = (await LoadAsync()).ToList();
            if (entity.Id == 0)
            {
                var max = list.Any() ? list.Max(b => b.Id) : 0;
                entity.Id = max + 1;
            }
            list.Add(entity);
            await SaveAsync(list);
            await BroadcastStatsUpdate();
            return entity;
        }

        public async Task<BookCopy> UpdateAsync(BookCopy entity)
        {
            var list = (await LoadAsync()).ToList();
            var idx = list.FindIndex(bc => bc.Id == entity.Id);
            if (idx == -1) throw new KeyNotFoundException();

            var original = list[idx];
            // Set LostDate when IsLost changes from false to true
            if (original.IsLost != true && entity.IsLost == true)
                entity.LostDate = System.DateTime.Now;
            // Clear LostDate when IsLost changes from true to false
            else if (original.IsLost == true && entity.IsLost != true)
                entity.LostDate = null;

            list[idx] = entity;
            await SaveAsync(list);
            await BroadcastStatsUpdate();
            return entity;
        }

        public async Task<bool> DeleteAsync(object key)
        {
            var list = (await LoadAsync()).ToList();
            var removed = list.RemoveAll(bc => bc.Id.Equals(key)) > 0;
            if (removed) {
                await SaveAsync(list);
                await BroadcastStatsUpdate();
            }
            return removed;
        }

        public async Task<int> GetLostBooksByHourForTodayAsync(int hour)
        {
            var today = System.DateTime.Today;
            var start = today.AddHours(hour);
            var end = start.AddHours(1);
            var list = await LoadAsync();
            return list.Count(bc => bc.LostDate != null && bc.LostDate >= start && bc.LostDate < end);
        }

        public async Task<int[]> GetLostBooksHourlyDataForTodayAsync()
        {
            var today = System.DateTime.Today;
            var hourly = new int[24];
            var list = await LoadAsync();
            var losts = list.Where(bc => bc.LostDate != null && bc.LostDate >= today && bc.LostDate < today.AddDays(1)).Select(bc => bc.LostDate!.Value);
            foreach (var d in losts) hourly[d.Hour]++;
            return hourly;
        }

        private async Task BroadcastStatsUpdate()
        {
            if (_hubContext == null) return;
            var total = await GetTotalCountAsync();
            var lost = await GetLostCountAsync();
            var hourly = await GetLostBooksHourlyDataForTodayAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveStatsUpdate", total, lost);
            await _hubContext.Clients.All.SendAsync("ReceiveHourlyChartUpdate", hourly);
        }
    }
}
