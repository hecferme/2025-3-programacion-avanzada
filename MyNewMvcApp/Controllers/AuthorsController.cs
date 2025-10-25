using Microsoft.AspNetCore.Mvc;
using ProgramacionAvanzada.Books.Repositories;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using MyNewMvcApp.Models;

namespace MyNewMvcApp.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _repo;
        public AuthorsController(IAuthorRepository repo) => _repo = repo;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string name)
        {
            using var client = new HttpClient();
            var apiUrl = Url.Action("Get", "Authors", new { name }, Request.Scheme) ?? $"/api/authors?name={name}";
            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "API call failed.";
                return View("Index", new List<AuthorDto>());
            }
            var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
            return View("Index", authors ?? new List<AuthorDto>());
        }
    }
}
