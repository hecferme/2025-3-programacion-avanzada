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
        private readonly string _booksFilePath;
        private readonly string _bookAuthorsFilePath;
        public JsonAuthorRepository(string filePath)
        {
            _jsonFilePath = filePath;
            var dir = Directory.GetParent(_jsonFilePath)!.FullName;
            _booksFilePath = Path.Combine(dir, "books.json");
            _bookAuthorsFilePath = Path.Combine(dir, "bookauthors.json");
        }
        private async Task<List<Author>> LoadAsync() => JsonSerializer.Deserialize<List<Author>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new();
        private async Task<List<Author>> LoadWithBooksAsync()
        {
            var authors = await LoadAsync();
            var books = JsonSerializer.Deserialize<List<Book>>(await File.ReadAllTextAsync(_booksFilePath)) ?? new();
            var bookAuthors = JsonSerializer.Deserialize<List<BookAuthor>>(await File.ReadAllTextAsync(_bookAuthorsFilePath)) ?? new();
            foreach (var author in authors)
            {
                var bookIds = bookAuthors.Where(ba => ba.AuthorId == author.Id).Select(ba => ba.BookId).ToList();
                author.Books = books.Where(b => bookIds.Contains(b.Id)).ToList();
            }
            return authors;
        }
        public async Task<Author?> GetByPrimaryKeyAsync(object key) => (await LoadWithBooksAsync()).FirstOrDefault(a => a.Id.Equals(key));
        public async Task<IEnumerable<Author>> GetByNameAsync(string name) => (await LoadWithBooksAsync()).Where(a => a.Name != null && a.Name.ToLower().Contains(name.ToLower()));
        public async Task<(IEnumerable<Author> Items, int TotalCount)> GetPagedAsync(string name, int pageNumber, int pageSize)
        {
            var allAuthors = (await LoadWithBooksAsync()).Where(a => a.Name != null && a.Name.ToLower().Contains(name.ToLower())).ToList();
            var totalCount = allAuthors.Count;
            var items = allAuthors.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (items, totalCount);
        }
        public async Task<IEnumerable<Author>> GetByCountryAsync(string country) => (await LoadWithBooksAsync()).Where(a => a.Country != null && a.Country.ToLower().Contains(country.ToLower()));
        public async Task<Author> InsertAsync(Author entity)
        {
            var list = (await LoadAsync()).ToList();
            list.Add(entity);
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }
        public async Task<Author> UpdateAsync(Author entity)
        {
            var list = (await LoadAsync()).ToList();
            var idx = list.FindIndex(a => a.Id.Equals(entity.Id));
            if (idx == -1) throw new KeyNotFoundException();
            list[idx] = entity;
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }
        public async Task<bool> DeleteAsync(object key)
        {
            var list = (await LoadAsync()).ToList();
            var removed = list.RemoveAll(a => a.Id.Equals(key)) > 0;
            if (removed)
                await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return removed;
        }
    }
}
