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
    public class ProductController : Controller
    {
        IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IUnitOfWork unit, IWebHostEnvironment environment)
        {
            unitOfWork = unit;
            webHostEnvironment = environment;
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
            List<Product> all_products = unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(all_products);
        }
        public IActionResult UpdateOrInsert(int? id)
        {
            ProductVM productVM = new() { CategoryList = GetListItemOfCategories(), Product = new Product() };
            
            if (id == null) return View(productVM);
            else
            {
                productVM.Product = unitOfWork.Product.GetFirstOrDefault(prod => prod.Id == id, includeProperties:"ProductImages")!;
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult UpdateOrInsert(ProductVM model, List<IFormFile> files)
        {
            // Validation
            if (model.Product.Title == model.Product.Author) ModelState.AddModelError("Title", "Title cannot exactly match the author");

            if (ModelState.IsValid)
            {
                // Product creating
                if (model.Product.Id == 0)
                {
                    unitOfWork.Product.Add(model.Product);
                }
                // Product updating
                else
                {
                    unitOfWork.Product.Update(model.Product);
                }
                unitOfWork.SaveChanges();

                // File image saving
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + model.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (Directory.Exists(finalPath) == false)
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (FileStream fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new ProductImage()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = model.Product.Id,
                        };

                        if (model.Product.ProductImages == null)
                        {
                            model.Product.ProductImages = new List<ProductImage>();
                        }

                        model.Product.ProductImages.Add(productImage);
                        unitOfWork.ProductImage.Add(productImage);
                    }
                    unitOfWork.SaveChanges();
                }
                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                TempData["error"] = "Not all fields are valid";
                ProductVM product_view_model = model;
                product_view_model.CategoryList = GetListItemOfCategories();
                return View(product_view_model);
            }
        }

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = unitOfWork.ProductImage.GetFirstOrDefault(image => image.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (string.IsNullOrEmpty(imageToBeDeleted.ImageUrl) == false)
                {
                    var old_image_path = Path.Combine(webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(old_image_path))
                    {
                        System.IO.File.Delete(old_image_path);
                    }
                }
                unitOfWork.ProductImage.Remove(imageToBeDeleted);
                unitOfWork.SaveChanges();
                TempData["success"] = "Deleted successfully";
            }
            return RedirectToAction("UpdateOrInsert", new { id = productId });
        }

        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> all_products = unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return Json(new { data = all_products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product_from_database = unitOfWork.Product.GetFirstOrDefault(prod => prod.Id == id);
            if (product_from_database == null) return Json(new { success = false, message = "No such product exists" });

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }

            unitOfWork.Product.Remove(product_from_database);
            unitOfWork.SaveChanges();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
