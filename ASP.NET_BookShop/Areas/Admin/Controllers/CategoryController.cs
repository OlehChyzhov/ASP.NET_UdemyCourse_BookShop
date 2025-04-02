using BookShop.DataAccess.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using BookShop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BookShop.Utility;

namespace ASP.NET_BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork unit)
        {
            unitOfWork = unit;
        }
        public IActionResult Index()
        {
            List<Category> all_categories = unitOfWork.Category.GetAll().ToList();
            return View(all_categories);
        }
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString()) ModelState.AddModelError("name", "The Display Order cannot exactly match the Name");
            if (ModelState.IsValid == false || category.Name == null) return View();

            unitOfWork.Category.Add(category);
            unitOfWork.SaveChanges();
            TempData["success"] = "Category created successfully";
            return RedirectToAction("Index", "Category");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            Category? category_from_database = unitOfWork.Category.GetFirstOrDefault(category => category.Id == id);

            if (category_from_database == null) return NotFound();
            return View(category_from_database);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString()) ModelState.AddModelError("name", "The Display Order cannot exactly match the Name");
            if (ModelState.IsValid == false || category.Name == null) return View();

            unitOfWork.Category.Update(category);
            unitOfWork.SaveChanges();
            TempData["success"] = "Category updated successfully";
            return RedirectToAction("Index", "Category");
        }
        public IActionResult Delete(int? id)
        {
            Category? category_from_database = unitOfWork.Category.GetFirstOrDefault(category => category.Id == id);
            if (category_from_database == null) return NotFound();

            unitOfWork.Category.Remove(category_from_database);
            unitOfWork.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index", "Category");
        }
    }
}
