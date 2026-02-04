using Microsoft.AspNetCore.Mvc;

namespace Quad_Solutions_Project.ApiService.Controllers
{
    public class TriviaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
