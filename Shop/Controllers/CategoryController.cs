using Microsoft.AspNetCore.Mvc;

namespace Shop.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
