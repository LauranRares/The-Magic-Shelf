// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TMS.Models;

namespace TMS.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<UserDB> _signInManager;
        private readonly UserManager<UserDB> _userManager;

        public LoginModel
        (
            SignInManager<UserDB> signInManager,
             UserManager<UserDB> userManager
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var check = await _userManager.FindByNameAsync(Input.Username);

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Books", "GuestViews", new { area = "Guest" });
                }
                else if (check != null)
                {
                    TempData["IncorrectPassword"] = "The password is incorrect";
                }
                else
                {
                    TempData["LogInFail"] = "Please make sure the info is correct";
                    return Page();
                }
            }

            return Page();
        }
    }
}
