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
    public class ChangeUsernameModel : PageModel
    {
        private readonly UserManager<UserDB> _userManager;
        private readonly SignInManager<UserDB> _signInManager;

        public ChangeUsernameModel
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
            public string NewUsername { get; set; }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var check = await _userManager.FindByNameAsync(Input.NewUsername);

            if (check != null)
            {
                TempData["Failed"] = "This username already exists";
                return Page();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);

                user.UserName = Input.NewUsername;

                await _userManager.UpdateAsync(user);

                await _signInManager.RefreshSignInAsync(user);

                TempData["Succeeded"] = "Username has been successfully changed";

                return LocalRedirect("/Identity/Account/Manage");

            }
        }
    }
}

