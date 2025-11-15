using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class JsonBookRepository : IBookRepository
    {
        private readonly string _jsonFilePath;
        private readonly string _authorsFilePath;
        private readonly string _themesFilePath;
        private readonly string _bookAuthorsFilePath;
        private readonly string _bookThemesFilePath;
        private readonly string _bookCopiesFilePath;

        public JsonBookRepository(string filePath)
        {
            _jsonFilePath = filePath;
            var dataDir = Path.GetDirectoryName(filePath) ?? "";
            _authorsFilePath = Path.Combine(dataDir, "authors.json");
            _themesFilePath = Path.Combine(dataDir, "themes.json");
            _bookAuthorsFilePath = Path.Combine(dataDir, "bookauthors.json");
            _bookThemesFilePath = Path.Combine(dataDir, "bookthemes.json");
            _bookCopiesFilePath = Path.Combine(dataDir, "bookcopies.json");
        }

        private async Task<List<Book>> LoadAsync() =>
            JsonSerializer.Deserialize<List<Book>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new();

        private async Task<List<Author>> LoadAuthorsAsync()
        {
            if (!File.Exists(_authorsFilePath)) return new();
            return JsonSerializer.Deserialize<List<Author>>(await File.ReadAllTextAsync(_authorsFilePath)) ?? new();
        }

        private async Task<List<Theme>> LoadThemesAsync()
        {
            if (!File.Exists(_themesFilePath)) return new();
            return JsonSerializer.Deserialize<List<Theme>>(await File.ReadAllTextAsync(_themesFilePath)) ?? new();
        }

        private async Task<List<BookAuthorLink>> LoadBookAuthorsAsync()
        {
            if (!File.Exists(_bookAuthorsFilePath)) return new();
            return JsonSerializer.Deserialize<List<BookAuthorLink>>(await File.ReadAllTextAsync(_bookAuthorsFilePath)) ?? new();
        }

        private async Task<List<BookThemeLink>> LoadBookThemesAsync()
        {
            if (!File.Exists(_bookThemesFilePath)) return new();
            return JsonSerializer.Deserialize<List<BookThemeLink>>(await File.ReadAllTextAsync(_bookThemesFilePath)) ?? new();
        }

        private async Task<List<BookCopy>> LoadBookCopiesAsync()
        {
            if (!File.Exists(_bookCopiesFilePath)) return new();
            return JsonSerializer.Deserialize<List<BookCopy>>(await File.ReadAllTextAsync(_bookCopiesFilePath)) ?? new();
        }

        private async Task PopulateNavigationPropertiesAsync(IEnumerable<Book> books)
        {
            var allAuthors = await LoadAuthorsAsync();
            var allThemes = await LoadThemesAsync();
            var bookAuthors = await LoadBookAuthorsAsync();
            var bookThemes = await LoadBookThemesAsync();
            var bookCopies = await LoadBookCopiesAsync();

            foreach (var book in books)
            {
                // Load authors
                var authorIds = bookAuthors.Where(ba => ba.BookId == book.Id).Select(ba => ba.AuthorId).ToList();
                book.Authors = allAuthors.Where(a => authorIds.Contains(a.Id)).ToList();

                // Load themes
                var themeIds = bookThemes.Where(bt => bt.BookId == book.Id).Select(bt => bt.ThemeId).ToList();
                book.Themes = allThemes.Where(t => themeIds.Contains(t.Id)).ToList();

                // Load book copies
                book.BookCopies = bookCopies.Where(bc => bc.BookId == book.Id).ToList();
            }
        }

        public async Task<Book?> GetByPrimaryKeyAsync(object key)
        {
            var books = (await LoadAsync()).Where(b => b.Id.Equals(key)).ToList();
            if (!books.Any()) return null;
            await PopulateNavigationPropertiesAsync(books);
            return books.FirstOrDefault();
        }

        public async Task<IEnumerable<Book>> GetByNameAsync(string name)
        {
            var books = (await LoadAsync()).Where(b =>
                (b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(name.ToLower())) ||
                (b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(name.ToLower()))).ToList();
            await PopulateNavigationPropertiesAsync(books);
            return books;
        }

        public async Task<(IEnumerable<Book> Items, int TotalCount)> GetPagedAsync(string name, int pageNumber, int pageSize)
        {
            var allBooks = (await LoadAsync()).Where(b =>
                (b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(name.ToLower())) ||
                (b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(name.ToLower()))).ToList();
            var totalCount = allBooks.Count;
            var pagedBooks = allBooks.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            await PopulateNavigationPropertiesAsync(pagedBooks);
            return (pagedBooks, totalCount);
        }

        public async Task<IEnumerable<Book>> GetByOriginalTitleAsync(string originalTitle)
        {
            var books = (await LoadAsync()).Where(b =>
                b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(originalTitle.ToLower())).ToList();
            await PopulateNavigationPropertiesAsync(books);
            return books;
        }

        public async Task<IEnumerable<Book>> GetByEnglishTitleAsync(string englishTitle)
        {
            var books = (await LoadAsync()).Where(b =>
                b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(englishTitle.ToLower())).ToList();
            await PopulateNavigationPropertiesAsync(books);
            return books;
        }
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

        // Helper classes for deserialization
        private class BookAuthorLink
        {
            [JsonPropertyName("BookId")]
            public int BookId { get; set; }
            [JsonPropertyName("AuthorId")]
            public int AuthorId { get; set; }
        }

        private class BookThemeLink
        {
            [JsonPropertyName("BookId")]
            public int BookId { get; set; }
            [JsonPropertyName("ThemeId")]
            public int ThemeId { get; set; }
        }
    }
}
