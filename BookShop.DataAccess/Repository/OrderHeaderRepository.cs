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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ShopContext shop_context;
        public OrderHeaderRepository(ShopContext context) : base(context) 
        { 
            shop_context = context; 
        }

        public void Update(OrderHeader order_header)
        {
            shop_context.OrderHeaders.Update(order_header);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = shop_context.OrderHeaders.FirstOrDefault(header => header.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (string.IsNullOrEmpty(paymentStatus) == false)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntendId)
        {
            var orderFromDb = shop_context.OrderHeaders.FirstOrDefault(header => header.Id == id);
            if (string.IsNullOrEmpty(sessionId) == false)
            {
                orderFromDb.SessionId = sessionId;
            }
            if (string.IsNullOrEmpty(paymentIntendId) == false)
            {
                orderFromDb.PaymentIntentId = paymentIntendId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
}
