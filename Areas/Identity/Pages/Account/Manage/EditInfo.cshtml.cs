using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using TMS.Models;


namespace TMS.Areas.Identity.Pages.Account.Manage
{
    public class EditInfoModel : PageModel
    {
        private readonly UserManager<UserDB> _userManager;
        private readonly SignInManager<UserDB> _signInManager;

        public EditInfoModel
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
            public string Name { get; set; }

            public string Location { get; set; }

            public string Address { get; set; }

            public string PhoneNumber { get; set; }

            public string CreditCard { get; set; }

            public string Pet { get; set; }
        }

        private async Task LoadAsync(UserDB user)
        {
            var _Name = user.Name;
            var _Location = user.Location;
            var _Address = user.Address;
            var _PhoneNr = user.PhoneNr;
            var _CreditCard = user.CreditCard;
            var _Pet = user.Pet;

            Input = new InputModel
            {
                Name = _Name,
                Location = _Location,
                Address = _Address,
                PhoneNumber = _PhoneNr,
                CreditCard = _CreditCard,
                Pet = _Pet
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                user.Name = Input.Name;
                user.Location = Input.Location;
                user.Address = Input.Address;
                user.PhoneNr = Input.PhoneNumber;
                user.CreditCard = Input.CreditCard;
                user.Pet = Input.Pet;

                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                TempData["Updated"] = "Info updated successfully";
                return Page();
            }
            else
            {
                TempData["Mistake"] = "Make sure all the data is correct";
                return Page();
            }
        }
    }
}
