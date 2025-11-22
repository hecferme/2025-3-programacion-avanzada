using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Repositories;

namespace MyNewMvcApp.Controllers
{
    public class BookCopiesStatsController : Controller
    {
        private readonly IBookCopyRepository _repository;

        public BookCopiesStatsController(IBookCopyRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var totalCount = await _repository.GetTotalCountAsync();
            var lostCount = await _repository.GetLostCountAsync();

            ViewBag.TotalCopies = totalCount;
            ViewBag.LostCopies = lostCount;
            ViewBag.AvailableCopies = totalCount - lostCount;

            return View();
        }
    }
}
