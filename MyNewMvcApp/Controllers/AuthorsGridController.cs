using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Repositories;
using ProgramacionAvanzada.Books.Model;
using System.Threading.Tasks;
using System.Linq;

namespace MyNewMvcApp.Controllers
{
    public class AuthorsGridController : Controller
    {
        private readonly IAuthorRepository _repo;

        public AuthorsGridController(IAuthorRepository repo)
        {
            _repo = repo;
        }

        // GET: AuthorsGrid
        public async Task<IActionResult> Index(string searchName)
        {
            var authors = string.IsNullOrWhiteSpace(searchName)
                ? await _repo.GetByNameAsync("")
                : await _repo.GetByNameAsync(searchName);

            ViewBag.SearchName = searchName;
            return View(authors);
        }

        // GET: AuthorsGrid/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var author = await _repo.GetByPrimaryKeyAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // GET: AuthorsGrid/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _repo.GetByPrimaryKeyAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: AuthorsGrid/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.UpdateAsync(author);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to save changes.");
                }
            }
            return View(author);
        }

        // GET: AuthorsGrid/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _repo.GetByPrimaryKeyAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: AuthorsGrid/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _repo.DeleteAsync(id);
            if (!result)
            {
                TempData["Error"] = "Unable to delete author.";
                return RedirectToAction(nameof(Delete), new { id });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
