using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccessLayer.Repository;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;
using Shop.Models.ViewModels;
using System.Security.Claims;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork UoW;
        public ShoppingCartVM ShoppingCartVM { get; set; } 

        public CartController( IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = UoW.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cartDb = UoW.ShoppingCart.Get(c => c.Id == cartId);
            cartDb.Count += 1;
            UoW.ShoppingCart.Update(cartDb);
            UoW.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartDb = UoW.ShoppingCart.Get(c => c.Id == cartId);
            if (cartDb.Count <= 1)
            {
                UoW.ShoppingCart.Delete(cartDb);
            }
            else
            {
                cartDb.Count -= 1;
                UoW.ShoppingCart.Update(cartDb);
            }

            UoW.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Remove(int cartId)
        {
            var cartDb = UoW.ShoppingCart.Get(c => c.Id == cartId);
            if (cartDb != null)
            {
                UoW.ShoppingCart.Delete(cartDb);
                UoW.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = UoW.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = UoW.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppincCart)
        {
            if (shoppincCart.Count <= 50)
            {
                return shoppincCart.Product.Price;
            }
            else if (shoppincCart.Count <= 100)
            {
                return shoppincCart.Product.Price50;
            }
            else
            {
                return shoppincCart.Product.Price100;
            }
        }
    }
}
