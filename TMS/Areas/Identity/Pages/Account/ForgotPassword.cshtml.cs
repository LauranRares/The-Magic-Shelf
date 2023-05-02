// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using TMS.Models;

namespace TMS.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly SignInManager<UserDB> _signInManager;
        private readonly UserManager<UserDB> _userManager;

        public ForgotPasswordModel
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

            public string Pet { get; set; }

            [StringLength(100, MinimumLength = 5)]
            public string NewPassword { get; set; }

            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(Input.Username);

            if (user != null)
            {
                if (user.Pet == Input.Pet)
                {

                    if (Input.NewPassword == Input.ConfirmPassword)
                    {
                        if ((Input.NewPassword).Length < 5)
                        {
                            TempData["ResetFailed3"] = "Password must contain at least 5 characters.";
                            return Page();
                        }
                        else
                        {
                            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                            var result = await _userManager.ResetPasswordAsync(user, resetToken, Input.NewPassword);

                            if (result.Succeeded)
                            {
                                TempData["ResetSuccessfully"] = "The password has been succesfully changed!";

                                await _signInManager.RefreshSignInAsync(user);

                                return Page();
                            }
                            else
                            {
                                TempData["ResetFailed2"] = "Password must contain at least an uppercase, a number and a special character.";
                                return Page();
                            }
                        }
                    }
                    else
                    {
                        TempData["ResetFailed"] = "Passwords must match.";
                        return Page();
                    }

                }
                else
                {
                    TempData["ResetFailed"] = "That's not the pet name.";
                    return Page();
                }
            }
            else
            {
                TempData["ResetFailed"] = "This user doesn't exists.";
                return Page();
            }
        }
    }
}
