using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ShopContext shop_context;
        public ProductRepository(ShopContext context) : base(context)
        {
            this.shop_context = context;
        }
        public void Update(Product product)
        {
            shop_context.Products.Update(product);
        }
    }
}
