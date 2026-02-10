using Microsoft.AspNetCore.Mvc;

namespace Bogdanovich.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
