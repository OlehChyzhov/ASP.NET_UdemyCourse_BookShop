using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using BookShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        ShopContext _shopContext;
        UserManager<IdentityUser> _userManager;
        public UserController(ShopContext context, UserManager<IdentityUser> userManager)
        {
            _shopContext = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string RoleId = _shopContext.UserRoles.FirstOrDefault(role => role.UserId == userId).RoleId;
            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                UserExtention = _shopContext.Users.Include("Company").FirstOrDefault(user => user.Id == userId),
                RoleList = _shopContext.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _shopContext.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.UserExtention.Role = _shopContext.Roles.FirstOrDefault(role => role.Id == RoleId).Name;
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string roleId = _shopContext.UserRoles.FirstOrDefault(role => role.UserId == roleManagementVM.UserExtention.Id).RoleId;
            string oldRole = _shopContext.Roles.FirstOrDefault(role => role.Id == roleId).Name;

            if (roleManagementVM.UserExtention.Role != oldRole)
            {
                // Role was updated
                UserExtention user = _shopContext.Users.FirstOrDefault(user => user.Id == roleManagementVM.UserExtention.Id);
                if (roleManagementVM.UserExtention.Role == SD.Role_Company)
                {
                    user.CompanyID = roleManagementVM.UserExtention.CompanyID;
                }
                if (oldRole == SD.Role_Company)
                {
                    user.CompanyID = null;
                }
                _shopContext.SaveChanges();

                _userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, roleManagementVM.UserExtention.Role).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }

        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<UserExtention> all_users = _shopContext.Users.Include("Company").ToList();
            var userRoles = _shopContext.UserRoles.ToList();
            var roles = _shopContext.Roles.ToList();

            foreach (UserExtention user in all_users)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.Company == null) user.Company = new Company() { Name = "None" };
            }

            return Json(new { data = all_users });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            string MESSAGE = "";
            var user = _shopContext.Users.FirstOrDefault(user => user.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                // User is locked. Unlocking him
                user.LockoutEnd = DateTime.Now;
                MESSAGE = "Unlocking Successful!";
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
                MESSAGE = "Locking Successfull!";
            }
            _shopContext.SaveChanges();
            return Json(new { success = true, message = MESSAGE });
        }
        #endregion
    }
}
