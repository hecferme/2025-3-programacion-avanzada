using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Model;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;
using System;

namespace MyNewMvcApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookThemesController : ControllerBase
    {
        private readonly IBookThemeRepository _repo;
        public BookThemesController(IBookThemeRepository repo) => _repo = repo;

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
        public async Task<IActionResult> Get([FromQuery] string? bookName, [FromQuery] string? themeName, [FromQuery] string? subjectName)
        {
            if (!string.IsNullOrWhiteSpace(bookName)) return Ok(await _repo.GetByBookNameAsync(bookName));
            if (!string.IsNullOrWhiteSpace(themeName)) return Ok(await _repo.GetByThemeNameAsync(themeName));
            if (!string.IsNullOrWhiteSpace(subjectName)) return Ok(await _repo.GetBySubjectNameAsync(subjectName));
            return Ok(await _repo.GetByNameAsync(""));
        }

        [HttpGet("{bookId}/{themeId}")]
        public async Task<IActionResult> GetByKey(int bookId, int themeId)
        {
            var entity = await _repo.GetByPrimaryKeyAsync((bookId, themeId));
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookTheme bt)
        {
            var created = await _repo.InsertAsync(bt);
            return CreatedAtAction(nameof(GetByKey), new { bookId = created.BookId, themeId = created.ThemeId }, created);
        }

        [HttpPut("{bookId}/{themeId}")]
        public async Task<IActionResult> Update(int bookId, int themeId, [FromBody] BookTheme bt)
        {
            if (bookId != bt.BookId || themeId != bt.ThemeId) return BadRequest();
            var updated = await _repo.UpdateAsync(bt);
            return Ok(updated);
        }

        [HttpDelete("{bookId}/{themeId}")]
        public async Task<IActionResult> Delete(int bookId, int themeId)
        {
            var removed = await _repo.DeleteAsync((bookId, themeId));
            return removed ? NoContent() : NotFound();
        }
    }
}
