// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TMS.Models;
using TMS.Roles;

namespace TMS.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UserDB> _signInManager;
        private readonly UserManager<UserDB> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<UserDB> _userStore;

        public RegisterModel
        (
            UserManager<UserDB> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<UserDB> userStore,
            SignInManager<UserDB> signInManager
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _signInManager = signInManager;
        }


        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {
            public string Username { get; set; }

            [StringLength(100, MinimumLength = 5)]
            public string Password { get; set; }

            public string ConfirmPassword { get; set; }

            public string Pet { get; set; }
        }

        public async Task OnGetAsync()
        {
            var check = await _roleManager.RoleExistsAsync(TheRoles._Admin);
            if (check == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(TheRoles._Admin));
                await _roleManager.CreateAsync(new IdentityRole(TheRoles._User));
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Input.Password == Input.ConfirmPassword)
            {
                var newuser = CreateUser();

                var check1 = await _userManager.FindByNameAsync(Input.Username);

                await _userStore.SetUserNameAsync(newuser, Input.Username, CancellationToken.None);
                var check2 = await _userManager.CreateAsync(newuser, Input.Password);

                if (check1 != null)
                {
                    TempData["UsernameExists"] = "This username already exists.";
                    return Page();
                }
                else if (check2.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newuser, TheRoles._User);

                    await _signInManager.SignInAsync(newuser, isPersistent: false);

                    newuser.Pet = Input.Pet;
                    await _userManager.UpdateAsync(newuser);

                    TempData["Registered"] = "Your registration was successful";
                    return LocalRedirect("/Identity/Account/Manage");
                }
                else if ((Input.Password).Length < 5)
                {
                    TempData["PasswordError"] = "Password must contain at least 5 characters.";
                    return Page();
                }
                else
                {
                    TempData["PasswordInfo"] = "Password must contain at least an uppercase, a number and a special character.";
                    return Page();
                }
            }
            else
            {
                TempData["PasswordError"] = "Passwords must match.";
                return Page();
            }

        }

        private UserDB CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UserDB>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserDB)}'. " +
                    $"Ensure that '{nameof(UserDB)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}
