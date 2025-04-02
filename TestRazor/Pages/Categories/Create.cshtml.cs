using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestRazor.Data;
using TestRazor.Models;

namespace TestRazor.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ShopContext shop_context;
        public Category Category { get; set; }
        public CreateModel(ShopContext context)
        {
            shop_context = context;
        }
        public IActionResult OnPost()
        {
            shop_context.Categories.Add(Category);
            shop_context.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
