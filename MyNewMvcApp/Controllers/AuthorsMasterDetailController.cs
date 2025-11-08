using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace MyNewMvcApp.Controllers
{
    public class AuthorsMasterDetailController : Controller
    {
        private readonly IAuthorRepository _repo;
        public AuthorsMasterDetailController(IAuthorRepository repo) => _repo = repo;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string name)
        {
            var authors = await _repo.GetByNameAsync(name ?? "");
            // return a partial view with the list of authors
            return PartialView("_AuthorList", authors);
        }

        [HttpGet]
        public async Task<IActionResult> Books(int id)
        {
            var author = await _repo.GetByPrimaryKeyAsync(id);
            if (author == null) return NotFound();
            return PartialView("_AuthorBooks", author);
        }
    }
}
