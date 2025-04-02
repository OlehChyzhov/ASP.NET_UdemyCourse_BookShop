using BookShop.DataAccess.Repository.IRepository;
using BookShop.DataAccess.Data;
using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ShopContext shop_context;
        public CategoryRepository(ShopContext context) : base(context) 
        { 
            shop_context = context; 
        }

        public void Update(Category category)
        {
            shop_context.Categories.Update(category);
        }
    }
}
