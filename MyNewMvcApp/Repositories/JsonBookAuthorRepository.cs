using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace ProgramacionAvanzada.Books.Repositories
{
    public class JsonBookAuthorRepository : IBookAuthorRepository
    {
        private readonly string _jsonFilePath;
        private readonly string _booksFilePath;
        private readonly string _authorsFilePath;
        public JsonBookAuthorRepository(string filePath, string booksFile, string authorsFile) { _jsonFilePath = filePath; _booksFilePath = booksFile; _authorsFilePath = authorsFile; }
        private async Task<List<BookAuthor>> LoadAsync() => JsonSerializer.Deserialize<List<BookAuthor>>(await File.ReadAllTextAsync(_jsonFilePath)) ?? new();
        private async Task<List<Book>> LoadBooksAsync() => JsonSerializer.Deserialize<List<Book>>(await File.ReadAllTextAsync(_booksFilePath)) ?? new();
        private async Task<List<Author>> LoadAuthorsAsync() => JsonSerializer.Deserialize<List<Author>>(await File.ReadAllTextAsync(_authorsFilePath)) ?? new();
        public async Task<BookAuthor?> GetByPrimaryKeyAsync(object key)
        {
            if (key is ValueTuple<int, int> tuple)
                return (await LoadAsync()).FirstOrDefault(ba => ba.BookId == tuple.Item1 && ba.AuthorId == tuple.Item2);
            return null;
        }
        public async Task<IEnumerable<BookAuthor>> GetByNameAsync(string name) => await GetByBookNameAsync(name);
        public async Task<IEnumerable<BookAuthor>> GetByBookNameAsync(string bookName)
        {
            var bas = await LoadAsync();
            var books = await LoadBooksAsync();
            return bas.Where(ba => books.Any(b => (b.Id == ba.BookId) && ((b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(bookName.ToLower())) || (b.EnglishTitle != null && b.EnglishTitle.ToLower().Contains(bookName.ToLower())))));
        }
        public async Task<IEnumerable<BookAuthor>> GetByAuthorNameAsync(string authorName)
        {
            var bas = await LoadAsync();
            var authors = await LoadAuthorsAsync();
            return bas.Where(ba => authors.Any(a => a.Id == ba.AuthorId && a.Name != null && a.Name.ToLower().Contains(authorName.ToLower())));
        }
        public async Task<BookAuthor> InsertAsync(BookAuthor entity)
        {
            var list = (await LoadAsync()).ToList();
            list.Add(entity);
            await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
            return entity;
        }
        public async Task<BookAuthor> UpdateAsync(BookAuthor entity)
        {
            var list = (await LoadAsync()).ToList();
            var idx = list.FindIndex(ba => ba.BookId == entity.BookId && ba.AuthorId == entity.AuthorId);
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
                var removed = list.RemoveAll(ba => ba.BookId == tuple.Item1 && ba.AuthorId == tuple.Item2) > 0;
                if (removed)
                    await File.WriteAllTextAsync(_jsonFilePath, JsonSerializer.Serialize(list));
                return removed;
            }
            return false;
        }
    }
}
