using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Data;
using TMS.Models;

namespace TMS.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        public List<TheShoppingCart> cartList = new List<TheShoppingCart>();

        private readonly TMSDbContext _db;

        public ShoppingCartViewComponent(TMSDbContext db)
        {
            _db = db;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userclaim = (ClaimsIdentity)User.Identity;
            var userId = userclaim.FindFirst(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return View("Default", false);
            }

            cartList = _db.DbCart.Where(x => x.TheUserId == userId.Value).ToList();

            if (cartList == null)
            {
                return View("Default", false);
            }

            if (cartList.Count > 0)
            {
                return View("Default", true);
            }
            else
            {
                return View("Default", false);
            }
        }
    }
}
