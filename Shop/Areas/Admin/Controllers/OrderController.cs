using Microsoft.AspNetCore.Mvc;

namespace Shop.Areas.Admin.Controllers
{
	public class OrderController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
