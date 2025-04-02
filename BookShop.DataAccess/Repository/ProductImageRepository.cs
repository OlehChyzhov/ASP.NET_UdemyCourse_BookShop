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
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private ShopContext shop_context;
        public ProductImageRepository(ShopContext context) : base(context) 
        { 
            shop_context = context; 
        }

        public void Update(ProductImage productImage)
        {
            shop_context.ProductImages.Update(productImage);
        }
    }
}
