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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ShopContext shop_context;
        public OrderDetailRepository(ShopContext context) : base(context) 
        { 
            shop_context = context; 
        }

        public void Update(OrderDetail order_detail)
        {
            shop_context.OrderDetails.Update(order_detail);
        }
    }
}
