// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TMS.Data;
using TMS.Models;

namespace TMS.Areas.Identity.Pages.Account.Manage
{
    public class DeleteAccountModel : PageModel
    {
        private readonly UserManager<UserDB> _userManager;
        private readonly SignInManager<UserDB> _signInManager;
        private readonly TMSDbContext _db;

        public DeleteAccountModel
        (
            UserManager<UserDB> userManager,
            SignInManager<UserDB> signInManager,
            TMSDbContext db
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Password { get; set; }
        }


        public async Task<IActionResult> OnPostAsync()
        {

            var user = await _userManager.GetUserAsync(User);

            if (!await _userManager.CheckPasswordAsync(user, Input.Password))
            {
                TempData["Fail"] = "The password is incorrect.";
                return Page();
            }


            var userclaim = (ClaimsIdentity)User.Identity;
            var userId = userclaim.FindFirst(ClaimTypes.NameIdentifier);

            List<TheShoppingCart> list1 = _db.DbCart.Where(x => x.TheUserId == userId.Value).ToList();
            _db.DbCart.RemoveRange(list1);
            _db.SaveChanges();

            List<UserHistory> list2 = _db.DbHistory.Where(x => x.TheUserId == userId.Value).ToList();
            _db.DbHistory.RemoveRange(list2);
            _db.SaveChanges();


            var delete = await _userManager.DeleteAsync(user);

            if (delete.Succeeded)
            {
                TempData["Deleted"] = "Account successfully deleted.";
                await _signInManager.SignOutAsync();
                return Redirect("~/");
            }

            return Page();
        }
    }
}
