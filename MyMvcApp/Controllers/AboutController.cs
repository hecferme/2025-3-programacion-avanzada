using Microsoft.AspNetCore.Mvc;

namespace MyMvcApp.Controllers
{
    public class MyMvcAppController : Controller
    {
        public IActionResult About()
        {
            var now = DateTime.Now;
            string formattedDate = now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewData["CurrentDate"] = formattedDate;
            return View();
        }
    }
}
