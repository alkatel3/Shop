using Microsoft.AspNetCore.Mvc;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;
using System.Diagnostics;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork UoW;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            UoW = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = UoW.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}