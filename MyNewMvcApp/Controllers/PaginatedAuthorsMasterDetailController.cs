using Microsoft.AspNetCore.Mvc;

namespace MyNewMvcApp.Controllers
{
    public class PaginatedAuthorsMasterDetailController : Controller
    {
        public IActionResult Index()
        {
            // The paginated page uses the Authors API paged endpoints;
            // this controller only serves the paginated layout view.
            return View();
        }
    }
}
