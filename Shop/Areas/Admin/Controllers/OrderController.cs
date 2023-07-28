using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccessLayer.Repository;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;
using Shop.Utility;
using System.Diagnostics;

namespace Shop.Areas.Admin.Controllers
{
	[Area("admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork UoW;

		public OrderController(IUnitOfWork _unitOfWork)
		{
			UoW = _unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders = UoW.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
					orderHeaders = orderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
