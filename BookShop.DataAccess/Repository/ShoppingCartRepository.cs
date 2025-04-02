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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private ShopContext shop_context;
        public ShoppingCartRepository(ShopContext context) : base(context) 
        { 
            shop_context = context; 
        }

        public void Update(ShoppingCart shopping_cart)
        {
            shop_context.ShoppingCarts.Update(shopping_cart);
        }
    }
}
