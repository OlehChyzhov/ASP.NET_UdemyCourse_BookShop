using BookShop.DataAccess.Data;
using BookShop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Models;

namespace BookShop.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        UserManager<IdentityUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        ShopContext shop_context;

        public DbInitializer(UserManager<IdentityUser> user, RoleManager<IdentityRole> role, ShopContext context)
        {
            _userManager = user;
            _roleManager = role;
            shop_context = context;
        }

        public void Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (shop_context.Database.GetPendingMigrations().Count() > 0)
                {
                    shop_context.Database.Migrate();
                }
            }
            catch (Exception e) {}

            //create roles if they are not created
            if (_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult() == false)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well
                _userManager.CreateAsync(new UserExtention
                {
                    UserName = "Admin@gmail.com",
                    Email = "Admin@gmail.com",
                    Name = "Test Admin",
                    PhoneNumber = "+380637219499",
                    StreetAddress = "Test 123 Ave",
                    State = "IL",
                    PostalCode = "23442",
                    City = "Lviv"
                }, "_Admin1_").GetAwaiter().GetResult();

                UserExtention user = shop_context.Users.FirstOrDefault(user => user.Email == "Admin@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
        }
    }
}
