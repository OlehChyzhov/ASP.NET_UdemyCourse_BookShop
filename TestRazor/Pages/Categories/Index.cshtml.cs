using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestRazor.Data;
using TestRazor.Models;

namespace TestRazor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ShopContext shop_context;
        public List<Category> CategoryList{ get; set; }
        public IndexModel(ShopContext context)
        {
            shop_context = context;
        }
        public void OnGet()
        {
            CategoryList = shop_context.Categories.ToList();
        }
    }
}
