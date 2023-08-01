using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccessLayer.Repository;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Utility;
using Stripe.Checkout;
using System.Security.Claims;

namespace Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork UoW;
        [BindProperty]
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
            var cartDb = UoW.ShoppingCart.Get(c => c.Id == cartId, tracked: true);
            if (cartDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                 UoW.ShoppingCart.GetAll(c => c.ApplicationUserId == cartDb.ApplicationUserId).Count()-1);
                UoW.ShoppingCart.Remove(cartDb);
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
            var cartDb = UoW.ShoppingCart.Get(c => c.Id == cartId, tracked:true);
            if (cartDb != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                UoW.ShoppingCart.GetAll(c => c.ApplicationUserId == cartDb.ApplicationUserId).Count()-1);
                UoW.ShoppingCart.Remove(cartDb);
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

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = UoW.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = UoW.ApplicationUser.Get(u => u.Id == userId);


			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            UoW.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            UoW.Save();
            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                UoW.OrderDetail.Add(orderDetail);
                UoW.Save();
            }

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
                var domain = "https://localhost:7293/";

                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach(var cart in ShoppingCartVM.ShoppingCartList)
                {
                    var seccionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(cart.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = cart.Product.Title
                            }
                        },
                        Quantity = cart.Count,
                    };
                    options.LineItems.Add(seccionLineItem);
				}
                var service = new SessionService();
                Session session = service.Create(options);

                UoW.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                UoW.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation),new {id=ShoppingCartVM.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = UoW.OrderHeader.Get(o => o.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
					UoW.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    UoW.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
					UoW.Save();
				}
			}

            List<ShoppingCart> shoppingCarts = UoW.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            UoW.ShoppingCart.RemoveRange(shoppingCarts);
            UoW.Save();
            return View(id);
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
