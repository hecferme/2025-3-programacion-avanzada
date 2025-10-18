using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Model;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;

namespace MyNewMvcApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _repo;
        public AuthorsController(IAuthorRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? country)
        {
            if (!string.IsNullOrWhiteSpace(country)) return Ok(await _repo.GetByCountryAsync(country));
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
