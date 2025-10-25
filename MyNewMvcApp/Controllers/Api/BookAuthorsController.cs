using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Model;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNewMvcApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookAuthorsController : ControllerBase
    {
        private readonly IBookAuthorRepository _repo;
        public BookAuthorsController(IBookAuthorRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? bookName, [FromQuery] string? authorName)
        {
            var result = new List<object>();
            IEnumerable<BookAuthor> entities;
            if (!string.IsNullOrWhiteSpace(bookName))
                entities = await _repo.GetByBookNameAsync(bookName);
            else if (!string.IsNullOrWhiteSpace(authorName))
                entities = await _repo.GetByAuthorNameAsync(authorName);
            else
                entities = await _repo.GetByNameAsync("");

            foreach (var ba in entities)
            {
                result.Add(new {
                    ba.BookId,
                    ba.AuthorId,
                    BookName = ba.Book?.OriginalTitle ?? ba.Book?.EnglishTitle,
                    AuthorName = ba.Author?.Name
                });
            }
            return Ok(result);
        }

        [HttpGet("{bookId}/{authorId}")]
        public async Task<IActionResult> GetByKey(int bookId, int authorId)
        {
            var entity = await _repo.GetByPrimaryKeyAsync((bookId, authorId));
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookAuthor ba)
        {
            var created = await _repo.InsertAsync(ba);
            return CreatedAtAction(nameof(GetByKey), new { bookId = created.BookId, authorId = created.AuthorId }, created);
        }

        [HttpPut("{bookId}/{authorId}")]
        public async Task<IActionResult> Update(int bookId, int authorId, [FromBody] BookAuthor ba)
        {
            if (bookId != ba.BookId || authorId != ba.AuthorId) return BadRequest();
            var updated = await _repo.UpdateAsync(ba);
            return Ok(updated);
        }

        [HttpDelete("{bookId}/{authorId}")]
        public async Task<IActionResult> Delete(int bookId, int authorId)
        {
            var removed = await _repo.DeleteAsync((bookId, authorId));
            return removed ? NoContent() : NotFound();
        }
    }
}
