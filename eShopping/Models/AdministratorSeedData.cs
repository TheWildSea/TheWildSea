using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopping.Models
{
    public class AdministratorSeedData
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<AppUser> _userManager;

        public AdministratorSeedData(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task EnsureSeedDataAsync()
        {
            if (await _userManager.FindByEmailAsync("12345@mail.com") == null)
            {
                AppUser admin = new AppUser()
                {
                    UserName = "12345@mail.com",
                    Email = "12345@mail.com"
                };

                await _userManager.CreateAsync(admin, "Password123!");
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

                IdentityResult result = await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
