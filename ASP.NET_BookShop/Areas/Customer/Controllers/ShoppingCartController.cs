using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using BookShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP.NET_BookShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var user_id = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(cart => cart.UserId == user_id, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(image => image.ProductId == cart.Product.Id).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var user_id = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(cart => cart.UserId == user_id, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.UserExtention = _unitOfWork.UserExtentions.GetFirstOrDefault(u => u.Id == user_id);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.UserExtention.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.UserExtention.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.UserExtention.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.UserExtention.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.UserExtention.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.UserExtention.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var user_id = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == user_id, includeProperties:"Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.UserId = user_id;

			UserExtention user = _unitOfWork.UserExtentions.GetFirstOrDefault(u => u.Id == user_id)!;

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}

            if (user.CompanyID.GetValueOrDefault() != 0)
            {
                // it is a company user
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            else
            {
                // it is regular customer account
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.SaveChanges();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail detail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(detail);
                _unitOfWork.SaveChanges();
            }

			if (user.CompanyID.GetValueOrDefault() == 0)
			{
				// it is regular customer account and we need to capture payment
				// STRIPE LOGIC SHOULD HAVE BEEN HERE
			}

            return RedirectToAction("OrderConfirmation", new { id = ShoppingCartVM.OrderHeader.Id });
		}

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "UserExtention");
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(cart => cart.UserId == orderHeader.UserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            HttpContext.Session.Clear();

            _unitOfWork.SaveChanges();
            return View();
        }

		public IActionResult Plus(int cartId)
        {
            var cart_from_database = _unitOfWork.ShoppingCart.GetFirstOrDefault(cart => cart.Id == cartId);
            cart_from_database.Count += 1;
            _unitOfWork.ShoppingCart.Update(cart_from_database);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Book Added";

            return RedirectToAction("Index", "ShoppingCart");
        }
        public IActionResult Minus(int cartId)
        {
            var cart_from_database = _unitOfWork.ShoppingCart.GetFirstOrDefault(cart => cart.Id == cartId, tracked:true);
            if (cart_from_database.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.UserId == cart_from_database.UserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(cart_from_database);
                TempData["success"] = "Books Removed";
            }
            else
            {
                cart_from_database.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cart_from_database);
                TempData["success"] = "Book Removed";
            }
            _unitOfWork.SaveChanges();

            return RedirectToAction("Index", "ShoppingCart");
        }
        public IActionResult Remove(int cartId)
        {
            var cart_from_database = _unitOfWork.ShoppingCart.GetFirstOrDefault(cart => cart.Id == cartId, tracked:true);
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.UserId == cart_from_database.UserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(cart_from_database);
            _unitOfWork.SaveChanges();

            TempData["success"] = "All Books Removed";
            return RedirectToAction("Index", "ShoppingCart");
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shopping_cart)
        {
            switch (shopping_cart.Count)
            {
                case <= 50:
                    return shopping_cart.Product.Price;
                case <= 100:
                    return shopping_cart.Product.Price50;
                case > 100:
                    return shopping_cart.Product.Price100;
            }
        }
    }
}
