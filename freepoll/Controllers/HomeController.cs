using Microsoft.AspNetCore.Mvc;

namespace freepoll.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("swagger");
        }
    }
}