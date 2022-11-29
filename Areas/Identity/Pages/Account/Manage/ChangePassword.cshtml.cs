// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TMS.Models;

namespace TMS.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<UserDB> _userManager;
        private readonly SignInManager<UserDB> _signInManager;

        public ChangePasswordModel
        (
            UserManager<UserDB> userManager,
            SignInManager<UserDB> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {
            public string OldPassword { get; set; }

            [StringLength(100, MinimumLength = 5)]
            public string NewPassword { get; set; }

            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (Input.NewPassword == Input.ConfirmPassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.OldPassword))
                {
                    TempData["ChangedFailed"] = "The old password is incorrect.";
                    return Page();
                }
                if ((Input.NewPassword).Length < 5)
                {
                    TempData["ChangedFailed"] = "Password must contain at least 5 characters.";
                    return Page();
                }
                else if (Input.OldPassword == Input.NewPassword)
                {
                    TempData["ChangedFailed"] = "The new password cannot be the same as the old one.";
                    return Page();
                }
                else
                {
                    var result = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

                    if (result.Succeeded)
                    {
                        TempData["ChangedSuccessfully"] = "The password has been succesfully changed!";

                        await _signInManager.RefreshSignInAsync(user);

                        return Page();
                    }
                    else
                    {
                        TempData["ChangedFailed"] = "Password must contain at least an uppercase, a number and a special character.";
                        return Page();
                    }
                }
            }
            else
            {
                TempData["ChangedFailed"] = "Passwords must match.";
                return Page();
            }
        }
    }
}
