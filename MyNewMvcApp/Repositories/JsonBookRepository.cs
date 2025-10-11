using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class JsonBookRepository : IBookRepository
    {
        private readonly string _jsonFilePath;
        public JsonBookRepository(string filePath) { _jsonFilePath = filePath; }
        private async Task<List<Book>> LoadAsync() => JsonSerializer.Deserialize<List<Book>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new();
        public async Task<Book?> GetByPrimaryKeyAsync(object key) => (await LoadAsync()).FirstOrDefault(b => b.Id.Equals(key));
        public async Task<IEnumerable<Book>> GetByNameAsync(string name) => (await LoadAsync()).Where(b => (b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(name.ToLower())) || (b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(name.ToLower())));
        public async Task<IEnumerable<Book>> GetByOriginalTitleAsync(string originalTitle) => (await LoadAsync()).Where(b => b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(originalTitle.ToLower()));
        public async Task<IEnumerable<Book>> GetByEnglishTitleAsync(string englishTitle) => (await LoadAsync()).Where(b => b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(englishTitle.ToLower()));
        public async Task<Book> InsertAsync(Book entity)
        {
            var list = (await LoadAsync()).ToList();
            list.Add(entity);
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }
        public async Task<Book> UpdateAsync(Book entity)
        {
            var list = (await LoadAsync()).ToList();
            var idx = list.FindIndex(b => b.Id.Equals(entity.Id));
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
