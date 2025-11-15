using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class JsonBookThemeRepository : IBookThemeRepository
    {
        private readonly string _jsonFilePath;
        private readonly string _booksFilePath;
        private readonly string _themesFilePath;
        public JsonBookThemeRepository(string filePath, string booksFile, string themesFile) { _jsonFilePath = filePath; _booksFilePath = booksFile; _themesFilePath = themesFile; }
        private async Task<List<BookTheme>> LoadAsync() => JsonSerializer.Deserialize<List<BookTheme>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new();
        private async Task<List<Book>> LoadBooksAsync() => JsonSerializer.Deserialize<List<Book>>(await File.ReadAllTextAsync(_booksFilePath)) ?? new();
        private async Task<List<Theme>> LoadThemesAsync() => JsonSerializer.Deserialize<List<Theme>>(await File.ReadAllTextAsync(_themesFilePath)) ?? new();
        public async Task<BookTheme?> GetByPrimaryKeyAsync(object key)
        {
            if (key is ValueTuple<int, int> tuple)
                return (await LoadAsync()).FirstOrDefault(bt => bt.BookId == tuple.Item1 && bt.ThemeId == tuple.Item2);
            return null;
        }
        public async Task<IEnumerable<BookTheme>> GetByNameAsync(string name) => await GetByBookNameAsync(name);
        public async Task<(IEnumerable<BookTheme> Items, int TotalCount)> GetPagedAsync(string name, int pageNumber, int pageSize)
        {
            var bts = await LoadAsync();
            var books = await LoadBooksAsync();
            var filtered = bts.Where(bt => books.Any(b => (b.Id == bt.BookId) && ((b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(name.ToLower())) || (b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(name.ToLower()))))).ToList();
            var totalCount = filtered.Count;
            var items = filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (items, totalCount);
        }
        public async Task<IEnumerable<BookTheme>> GetByBookNameAsync(string bookName)
        {
            var bts = await LoadAsync();
            var books = await LoadBooksAsync();
            return bts.Where(bt => books.Any(b => (b.Id == bt.BookId) && ((b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(bookName.ToLower())) || (b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(bookName.ToLower())))));
        }
        public async Task<IEnumerable<BookTheme>> GetByThemeNameAsync(string themeName)
        {
            var bts = await LoadAsync();
            var themes = await LoadThemesAsync();
            return bts.Where(bt => themes.Any(t => t.Id == bt.ThemeId && t.ThemeName != null && t.ThemeName.ToLower().Contains(themeName.ToLower())));
        }
        public async Task<IEnumerable<BookTheme>> GetBySubjectNameAsync(string subjectName)
        {
            var bts = await LoadAsync();
            var themes = await LoadThemesAsync();
            return bts.Where(bt => themes.Any(t => t.Id == bt.ThemeId && t.Subject != null && t.Subject.ToLower().Contains(subjectName.ToLower())));
        }
        public async Task<BookTheme> InsertAsync(BookTheme entity)
        {
            var list = (await LoadAsync()).ToList();
            list.Add(entity);
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }
        public async Task<BookTheme> UpdateAsync(BookTheme entity)
        {
            var list = (await LoadAsync()).ToList();
            var idx = list.FindIndex(bt => bt.BookId == entity.BookId && bt.ThemeId == entity.ThemeId);
            if (idx == -1) throw new KeyNotFoundException();
            list[idx] = entity;
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }
        public async Task<bool> DeleteAsync(object key)
        {
            if (key is ValueTuple<int, int> tuple)
            {
                var list = (await LoadAsync()).ToList();
                var removed = list.RemoveAll(bt => bt.BookId == tuple.Item1 && bt.ThemeId == tuple.Item2) > 0;
                if (removed)
                    await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
                return removed;
            }
            return false;
        }
    }
}
