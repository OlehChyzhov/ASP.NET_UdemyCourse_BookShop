using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ShopContext shop_context;
        public CompanyRepository(ShopContext context) : base(context)
        {
            this.shop_context = context;
        }
        public void Update(Company company)
        {
            shop_context.Companies.Update(company);
        }
    }
}
