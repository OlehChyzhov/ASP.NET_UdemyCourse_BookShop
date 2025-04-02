using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using BookShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.NET_BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        IUnitOfWork unitOfWork;
        public CompanyController(IUnitOfWork unit)
        {
            unitOfWork = unit;
        }
        public IEnumerable<SelectListItem> GetListItemOfCategories()
        {
            IEnumerable<SelectListItem> CategoryList = unitOfWork.Category.GetAll().Select(category => new SelectListItem
            {
                Text = category.Name,
                Value = category.Id.ToString()
            });
            return CategoryList;
        }
        public IActionResult Index()
        {
            List<Company> all_products = unitOfWork.Company.GetAll().ToList();
            return View(all_products);
        }
        public IActionResult UpdateOrInsert(int? id)
        {   
            if (id == null) return View(new Company());
            else
            {
                Company company_from_database = unitOfWork.Company.GetFirstOrDefault(comp => comp.Id == id)!;
                return View(company_from_database);
            }
        }
        [HttpPost]
        public IActionResult UpdateOrInsert(Company company)
        {
            if (ModelState.IsValid)
            {
                // Product creating
                if (company.Id == 0)
                {
                    unitOfWork.Company.Add(company);
                    TempData["success"] = "Company created successfully";
                }
                // Product updating
                else
                {
                    unitOfWork.Company.Update(company);
                    TempData["success"] = "Company updated successfully";
                }
                unitOfWork.SaveChanges();
            }
            else
            {
                TempData["error"] = "Something went wrong";
                return View(company);
            }
            
            return RedirectToAction("Index", "Company");
        }

        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> all_companies = unitOfWork.Company.GetAll().ToList();
            return Json(new { data = all_companies });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var company_from_database = unitOfWork.Company.GetFirstOrDefault(comp => comp.Id == id);
            if (company_from_database == null) return Json(new { success = false, message = "No such company exists" });

            unitOfWork.Company.Remove(company_from_database);
            unitOfWork.SaveChanges();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
