using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramacionAvanzada.Books.Model;

namespace ProgramacionAvanzada.Books.Repositories
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Author>> GetByCountryAsync(string country);
    }

    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetByOriginalTitleAsync(string originalTitle);
        Task<IEnumerable<Book>> GetByEnglishTitleAsync(string englishTitle);
        Task<Book> InsertAsync(Book entity);
        Task<Book> UpdateAsync(Book entity);
        Task<bool> DeleteAsync(object key);
    }

    public interface IBookAuthorRepository : IRepository<BookAuthor>
    {
        Task<IEnumerable<BookAuthor>> GetByBookNameAsync(string bookName);
        Task<IEnumerable<BookAuthor>> GetByAuthorNameAsync(string authorName);
    }

    public interface IThemeRepository : IRepository<Theme>
    {
        Task<IEnumerable<Theme>> GetBySubjectAsync(string subject);
    }

    public interface IBookThemeRepository : IRepository<BookTheme>
    {
        Task<IEnumerable<BookTheme>> GetByBookNameAsync(string bookName);
        Task<IEnumerable<BookTheme>> GetByThemeNameAsync(string themeName);
        Task<IEnumerable<BookTheme>> GetBySubjectNameAsync(string subjectName);
    }

    public interface IBookCopyRepository : IRepository<BookCopy>
    {
        Task<IEnumerable<BookCopy>> GetAllAsync();
        Task<IEnumerable<BookCopy>> GetByIsLostAsync(bool isLost);
        Task<int> GetTotalCountAsync();
        Task<int> GetLostCountAsync();
    }
}
