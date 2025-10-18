using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Model;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;

namespace MyNewMvcApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThemesController : ControllerBase
    {
        private readonly IThemeRepository _repo;
        public ThemesController(IThemeRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? subject)
        {
            if (!string.IsNullOrWhiteSpace(subject)) return Ok(await _repo.GetBySubjectAsync(subject));
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
        public async Task<IActionResult> Create([FromBody] Theme theme)
        {
            var created = await _repo.InsertAsync(theme);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Theme theme)
        {
            if (id != theme.Id) return BadRequest();
            var updated = await _repo.UpdateAsync(theme);
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
