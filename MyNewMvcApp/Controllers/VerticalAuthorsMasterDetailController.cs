using Microsoft.AspNetCore.Mvc;

namespace MyNewMvcApp.Controllers
{
    public class VerticalAuthorsMasterDetailController : Controller
    {
        public IActionResult Index()
        {
            // The vertical page reuses the AuthorsMasterDetail controller endpoints
            // for Search and Books; this controller only serves the vertical layout view.
            return View();
        }
    }
}
