using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Model;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;
using System;

namespace MyNewMvcApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowsController : ControllerBase
    {
        private readonly IBorrowRepository _repo;
        public BorrowsController(IBorrowRepository repo) => _repo = repo;

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
        public async Task<IActionResult> Get([FromQuery] int? personId, [FromQuery] int? bookCopyId, [FromQuery] string? name)
        {
            if (personId.HasValue) return Ok(await _repo.GetByPersonIdAsync(personId.Value));
            if (bookCopyId.HasValue) return Ok(await _repo.GetByBookCopyIdAsync(bookCopyId.Value));
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
        public async Task<IActionResult> Create([FromBody] Borrow borrow)
        {
            var created = await _repo.InsertAsync(borrow);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Borrow borrow)
        {
            if (id != borrow.Id) return BadRequest();
            var updated = await _repo.UpdateAsync(borrow);
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
