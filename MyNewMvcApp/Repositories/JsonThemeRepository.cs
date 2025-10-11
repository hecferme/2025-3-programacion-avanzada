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
        public async Task<Theme> InsertAsync(Theme entity)
        {
            var list = (await LoadAsync()).ToList();
            list.Add(entity);
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }
        public async Task<Theme> UpdateAsync(Theme entity)
        {
            var list = (await LoadAsync()).ToList();
            var idx = list.FindIndex(t => t.Id.Equals(entity.Id));
            if (idx == -1) throw new KeyNotFoundException();
            list[idx] = entity;
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }
        public async Task<bool> DeleteAsync(object key)
        {
            var list = (await LoadAsync()).ToList();
            var removed = list.RemoveAll(t => t.Id.Equals(key)) > 0;
            if (removed)
                await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return removed;
        }
    }
}
