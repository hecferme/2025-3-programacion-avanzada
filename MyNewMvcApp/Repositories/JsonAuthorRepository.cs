using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class JsonAuthorRepository : IAuthorRepository
    {
        private readonly string _jsonFilePath;
        public JsonAuthorRepository(string filePath) { _jsonFilePath = filePath; }
        private async Task<List<Author>> LoadAsync() => JsonSerializer.Deserialize<List<Author>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new();
        public async Task<Author?> GetByPrimaryKeyAsync(object key) => (await LoadAsync()).FirstOrDefault(a => a.Id.Equals(key));
        public async Task<IEnumerable<Author>> GetByNameAsync(string name) => (await LoadAsync()).Where(a => a.Name != null && a.Name.ToLower().Contains(name.ToLower()));
        public async Task<IEnumerable<Author>> GetByCountryAsync(string country) => (await LoadAsync()).Where(a => a.Country != null && a.Country.ToLower().Contains(country.ToLower()));
    }
}
