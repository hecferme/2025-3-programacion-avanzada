using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Model;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;

namespace MyNewMvcApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repo;
        public BooksController(IBookRepository repo) => _repo = repo;

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] string? name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _repo.GetPagedAsync(name ?? "", pageNumber, pageSize);
            return Ok(new {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? originalTitle, [FromQuery] string? englishTitle)
        {
            if (!string.IsNullOrWhiteSpace(originalTitle)) return Ok(await _repo.GetByOriginalTitleAsync(originalTitle));
            if (!string.IsNullOrWhiteSpace(englishTitle)) return Ok(await _repo.GetByEnglishTitleAsync(englishTitle));
            if (!string.IsNullOrWhiteSpace(name)) return Ok(await _repo.GetByNameAsync(name));
            return Ok(await _repo.GetByNameAsync(""));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _repo.GetByPrimaryKeyAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            var created = await _repo.InsertAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            if (id != book.Id) return BadRequest();
            var updated = await _repo.UpdateAsync(book);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _repo.DeleteAsync(id);
            return removed ? NoContent() : NotFound();
        }
    }
}
