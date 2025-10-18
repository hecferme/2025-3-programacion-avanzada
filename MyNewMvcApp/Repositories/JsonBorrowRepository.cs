using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class JsonBorrowRepository : IBorrowRepository
    {
        private readonly string _jsonFilePath;
        private readonly string? _bookCopiesFile;
        private readonly string? _personsFile;

        public JsonBorrowRepository(string filePath, string? bookCopiesFile = null, string? personsFile = null)
        {
            _jsonFilePath = filePath;
            _bookCopiesFile = bookCopiesFile;
            _personsFile = personsFile;
        }

        private async Task<List<Borrow>> LoadAsync()
        {
            if (!File.Exists(_jsonFilePath)) return new List<Borrow>();
            return JsonSerializer.Deserialize<List<Borrow>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new List<Borrow>();
        }

        private async Task<List<Person>> LoadPersonsAsync()
        {
            if (string.IsNullOrWhiteSpace(_personsFile) || !File.Exists(_personsFile)) return new List<Person>();
            return JsonSerializer.Deserialize<List<Person>>(await File.ReadAllTextAsync(_personsFile)) ?? new List<Person>();
        }

        public async Task<Borrow?> GetByPrimaryKeyAsync(object key) => (await LoadAsync()).FirstOrDefault(b => b.Id.Equals(key));

        public async Task<IEnumerable<Borrow>> GetByNameAsync(string name)
        {
            var persons = await LoadPersonsAsync();
            if (persons.Count == 0) return Enumerable.Empty<Borrow>();
            var personIds = persons.Where(p => p.Name != null && p.Name.ToLower().Contains(name.ToLower())).Select(p => p.Id).ToHashSet();
            var borrows = await LoadAsync();
            return borrows.Where(b => personIds.Contains(b.PersonId));
        }

        public async Task<IEnumerable<Borrow>> GetByPersonIdAsync(int personId)
        {
            var borrows = await LoadAsync();
            return borrows.Where(b => b.PersonId == personId);
        }

        public async Task<IEnumerable<Borrow>> GetByBookCopyIdAsync(int bookCopyId)
        {
            var borrows = await LoadAsync();
            return borrows.Where(b => b.BookCopyId == bookCopyId);
        }

        public async Task<Borrow> InsertAsync(Borrow entity)
        {
            var list = (await LoadAsync()).ToList();
            // assign a new id if zero or colliding
            if (entity.Id == 0)
            {
                var max = list.Any() ? list.Max(b => b.Id) : 0;
                entity.Id = max + 1;
            }
            list.Add(entity);
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }

        public async Task<Borrow> UpdateAsync(Borrow entity)
        {
            var list = (await LoadAsync()).ToList();
            var idx = list.FindIndex(b => b.Id == entity.Id);
            if (idx == -1) throw new KeyNotFoundException();
            list[idx] = entity;
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }

        public async Task<bool> DeleteAsync(object key)
        {
            var list = (await LoadAsync()).ToList();
            var removed = list.RemoveAll(b => b.Id.Equals(key)) > 0;
            if (removed)
                await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return removed;
        }
    }
}
