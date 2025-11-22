using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Model;
using ProgramacionAvanzada.Books.Repositories;

namespace MyNewMvcApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookCopiesController : ControllerBase
    {
        private readonly IBookCopyRepository _repository;

        public BookCopiesController(IBookCopyRepository repository)
        {
            _repository = repository;
        }

        // GET: api/BookCopies
        // Optional header: isLost (1 = lost, 0 = available)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookCopy>>> GetBookCopies()
        {
            // Check if isLost header is present
            if (Request.Headers.TryGetValue("isLost", out var isLostValue))
            {
                if (int.TryParse(isLostValue, out int isLostInt))
                {
                    bool isLost = isLostInt == 1;
                    var filteredCopies = await _repository.GetByIsLostAsync(isLost);
                    return Ok(filteredCopies);
                }
                else
                {
                    return BadRequest("Invalid isLost header value. Use 1 for lost or 0 for available.");
                }
            }

            // No header provided, return all book copies
            var allCopies = await _repository.GetAllAsync();
            return Ok(allCopies);
        }

        // GET: api/BookCopies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookCopy>> GetBookCopy(int id)
        {
            var bookCopy = await _repository.GetByPrimaryKeyAsync(id);

            if (bookCopy == null)
            {
                return NotFound();
            }

            return Ok(bookCopy);
        }

        // POST: api/BookCopies
        [HttpPost]
        public async Task<ActionResult<BookCopy>> PostBookCopy(BookCopy bookCopy)
        {
            var created = await _repository.InsertAsync(bookCopy);
            return CreatedAtAction(nameof(GetBookCopy), new { id = created.Id }, created);
        }

        // PUT: api/BookCopies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookCopy(int id, BookCopy bookCopy)
        {
            if (id != bookCopy.Id)
            {
                return BadRequest();
            }

            var updated = await _repository.UpdateAsync(bookCopy);
            return Ok(updated);
        }

        // PATCH: api/BookCopies/5/lost
        // Update only the isLost field
        [HttpPatch("{id}/lost")]
        public async Task<IActionResult> UpdateIsLost(int id, [FromBody] bool isLost)
        {
            var bookCopy = await _repository.GetByPrimaryKeyAsync(id);
            if (bookCopy == null)
            {
                return NotFound();
            }

            bookCopy.IsLost = isLost;
            var updated = await _repository.UpdateAsync(bookCopy);
            return Ok(updated);
        }

        // DELETE: api/BookCopies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookCopy(int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/BookCopies/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStats()
        {
            var totalCount = await _repository.GetTotalCountAsync();
            var lostCount = await _repository.GetLostCountAsync();
            
            return Ok(new
            {
                totalCopies = totalCount,
                lostCopies = lostCount,
                availableCopies = totalCount - lostCount
            });
        }

        // GET: api/BookCopies/lost-by-hour/14
        [HttpGet("lost-by-hour/{hour}")]
        public async Task<ActionResult<object>> GetLostBooksByHour(int hour)
        {
            if (hour < 0 || hour > 23)
            {
                return BadRequest("Hour must be between 0 and 23.");
            }

            var count = await _repository.GetLostBooksByHourForTodayAsync(hour);
            return Ok(new { hour, count });
        }

        // GET: api/BookCopies/lost-hourly-today
        [HttpGet("lost-hourly-today")]
        public async Task<ActionResult<int[]>> GetLostBooksHourlyToday()
        {
            var hourlyData = await _repository.GetLostBooksHourlyDataForTodayAsync();
            return Ok(hourlyData);
        }
    }
}
