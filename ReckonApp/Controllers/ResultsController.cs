using Microsoft.AspNetCore.Mvc;

namespace ReckonApp.Controllers
{
    public class ResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
