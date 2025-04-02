using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestRazor.Data;
using TestRazor.Models;

namespace TestRazor.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ShopContext shop_context;
        public Category Category { get; set; }
        public EditModel(ShopContext context)
        {
            shop_context = context;
        }
        public void OnGet(int? id)
        {
            if (id != null && id != 0)
            {
                Category = shop_context.Categories.FirstOrDefault(category => category.Id == id);
            }
        }
        public IActionResult OnPost()
        {
            shop_context.Categories.Update(Category);
            shop_context.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
