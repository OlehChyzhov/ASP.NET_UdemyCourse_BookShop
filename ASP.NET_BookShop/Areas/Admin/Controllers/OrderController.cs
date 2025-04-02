using BookShop.DataAccess.Repository;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using BookShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ASP.NET_BookShop.Areas.Admin.Controllers
{
	[Area("admin")]
    [Authorize]
	public class OrderController : Controller
	{
		IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}
        public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(header => header.Id == orderId, includeProperties: "UserExtention"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(detail => detail.OrderHeaderId == orderId, includeProperties: "Product"),
            };
            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.City;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            if (string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier) == false)
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber) == false)
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Order Details Updated Successfully";

            return RedirectToAction("Details", new {orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Order Details Updated Successfully";

            return RedirectToAction("Details", new {orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(header => header.Id == OrderVM.OrderHeader.Id);

            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Order Shipped Successfully";

            return RedirectToAction("Details", new { orderId = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(header => header.Id == OrderVM.OrderHeader.Id);
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                // Give a refund
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.PaymentStatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.SaveChanges();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction("Details", new { orderId = OrderVM.OrderHeader.Id });
        }

        public IActionResult PaymentConformation()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader
                .GetFirstOrDefault(header => header.Id == OrderVM.OrderHeader.Id, includeProperties: "UserExtention")!;

            if (OrderVM.OrderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, OrderVM.OrderHeader.OrderStatus, SD.PaymentStatusApproved);
                _unitOfWork.SaveChanges();
            }
            return View(OrderVM.OrderHeader.Id);
        }

        #region APICALLS
        [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orders = _unitOfWork.OrderHeader.GetAll(includeProperties: "UserExtention").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orders = _unitOfWork.OrderHeader.GetAll(u=>u.UserId == UserId, includeProperties:"UserExtention").ToList();
            }

            switch (status)
            {
                case "pending":
                    orders = orders.Where(order => order.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    orders = orders.Where(order => order.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orders = orders.Where(order => order.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orders = orders.Where(order => order.OrderStatus == SD.StatusApproved);
                    break;
            }

            return Json(new { data = orders });
		}
		#endregion
	}
}
