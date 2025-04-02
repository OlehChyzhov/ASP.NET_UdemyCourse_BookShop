using BookShop.DataAccess.Repository;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP.NET_BookShop.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.UserId == claim.Value).Count());
                }
                
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
