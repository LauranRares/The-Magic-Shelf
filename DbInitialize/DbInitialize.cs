using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models;
using TMS.Roles;

namespace TMS.DbInitialize
{
    public class DbInitialize
    {
        private readonly UserManager<UserDB> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TMSDbContext _db;

        public DbInitialize
        (
            TMSDbContext db,
            UserManager<UserDB> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex ) 
            {

            }


            if (!_roleManager.RoleExistsAsync(TheRoles._Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(TheRoles._Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(TheRoles._User)).GetAwaiter().GetResult();


                _userManager.CreateAsync(new UserDB
                {
                    UserName = "ADMIN",
                    Pet = "GOD"
                }, "Admin_1").GetAwaiter().GetResult();

                UserDB user = _db.DbUsers.FirstOrDefault(x => x.UserName == "ADMIN");

                _userManager.AddToRoleAsync(user, TheRoles._Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}