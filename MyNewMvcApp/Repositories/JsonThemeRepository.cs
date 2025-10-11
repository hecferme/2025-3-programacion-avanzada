using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class JsonThemeRepository : IThemeRepository
    {
        private readonly string _jsonFilePath;
        public JsonThemeRepository(string filePath) { _jsonFilePath = filePath; }
        private async Task<List<Theme>> LoadAsync() => JsonSerializer.Deserialize<List<Theme>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new();
        public async Task<Theme?> GetByPrimaryKeyAsync(object key) => (await LoadAsync()).FirstOrDefault(t => t.Id.Equals(key));
        public async Task<IEnumerable<Theme>> GetByNameAsync(string name) => (await LoadAsync()).Where(t => t.ThemeName != null && t.ThemeName.ToLower().Contains(name.ToLower()));
        public async Task<IEnumerable<Theme>> GetBySubjectAsync(string subject) => (await LoadAsync()).Where(t => t.Subject != null && t.Subject.ToLower().Contains(subject.ToLower()));
    }
}
