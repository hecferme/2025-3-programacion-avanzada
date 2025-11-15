using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Model;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MyNewMvcApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _repo;
        public AuthorsController(IAuthorRepository repo) => _repo = repo;

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] string? name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _repo.GetPagedAsync(name ?? "", pageNumber, pageSize);
            var result = new List<object>();
            foreach (var author in items)
            {
                result.Add(new {
                    author.Id,
                    author.Name,
                    author.Country,
                    Books = author.Books?.Select(b => b.OriginalTitle ?? b.EnglishTitle).ToList() ?? new List<string?>()
                });
            }

            return Ok(new {
                Items = result,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? country)
        {
            var result = new List<object>();
            IEnumerable<Author> entities;
            if (!string.IsNullOrWhiteSpace(country))
                entities = await _repo.GetByCountryAsync(country);
            else if (!string.IsNullOrWhiteSpace(name))
                entities = await _repo.GetByNameAsync(name);
            else
                entities = await _repo.GetByNameAsync("");

            foreach (var author in entities)
            {
                result.Add(new {
                    author.Id,
                    author.Name,
                    author.Country,
                    Books = author.Books?.Select(b => b.OriginalTitle ?? b.EnglishTitle).ToList() ?? new List<string?>()
                });
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _repo.GetByPrimaryKeyAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Author author)
        {
            var created = await _repo.InsertAsync(author);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Author author)
        {
            if (id != author.Id) return BadRequest();
            var updated = await _repo.UpdateAsync(author);
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
