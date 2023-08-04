using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;
using Shop.Utility;
using System.Diagnostics;
using System.Security.Claims;

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
            IEnumerable<Product> productList = UoW.Product.GetAll(includeProperties: "Category,ProductImages");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = UoW.Product.Get(p => p.Id == id, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId =id
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            //Get Exception. From form I get cart.Id=cart.ProductId
            cart.Id = 0;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            ShoppingCart CardDb = UoW.ShoppingCart.Get(c =>   c.ApplicationUserId == userId &&
                                                            c.ProductId == cart.ProductId);
            if (CardDb != null)
            {
                CardDb.Count += cart.Count;
                UoW.ShoppingCart.Update(CardDb);
                UoW.Save();
            }
            else
            {
                UoW.ShoppingCart.Add(cart);
                UoW.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, 
                    UoW.ShoppingCart.GetAll(c => c.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully"
;


            return RedirectToAction("Index");
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