using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Security.Claims;
using TMS.Data;
using TMS.Models;
using TMS.Roles;

namespace TMS.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]

    public class CartActionsController : Controller
    {
        public List<TheShoppingCart> cartList = new List<TheShoppingCart>();
        public List<TheShoppingCart> cartList2 = new List<TheShoppingCart>();
        public List<UserHistory> historyList = new List<UserHistory>();

        // DB & UserManager
        private readonly UserManager<UserDB> _userManager;
        private readonly TMSDbContext _db;

        public CartActionsController
        (
            TMSDbContext db,
            UserManager<UserDB> userManager
        )
        {
            _db = db;
            _userManager = userManager;
        }

        //Views
        public IActionResult ShoppingCart()
        {
            var userclaim = (ClaimsIdentity)User.Identity;
            var userId = userclaim.FindFirst(ClaimTypes.NameIdentifier);

            cartList = _db.DbCart.Where(x => x.TheUserId == userId.Value).ToList();
            return View(cartList);
        }

        public IActionResult History()
        {
            if (User.IsInRole(TheRoles._Admin))
            {
                historyList = _db.DbHistory.ToList();
                return View(historyList);
            }
            else
            {
                var userclaim = (ClaimsIdentity)User.Identity;
                var userId = userclaim.FindFirst(ClaimTypes.NameIdentifier);

                historyList = _db.DbHistory.Where(x => x.TheUserId == userId.Value).ToList();
                return View(historyList);
            }
        }

        //Actions
        public IActionResult AddNow(int id)
        {
            var userclaim = (ClaimsIdentity)User.Identity;
            var userId = userclaim.FindFirst(ClaimTypes.NameIdentifier);

            var added = _db.DbBooks.Find(id);
            var check = _db.DbCart.Where(x => x.TheUserId == userId.Value).FirstOrDefault(y => y.BookId == added.Id);

            if (added.Quantity <= 0)
            {
                TempData["Info"] = "No more books available.";
                return RedirectToAction("Books", "GuestViews", new { area = "Guest" });
            }
            else if (check == null)
            {
                TheShoppingCart cart = new TheShoppingCart()
                {
                    BookId = added.Id,
                    TheUserId = userId.Value,
                    Title = added.Title,
                    Author = added.Author,
                    Quantity = 1,
                    Price = added.Price
                };
                _db.DbCart.Add(cart);
                _db.SaveChanges();
            }
            else
            {
                TempData["Info"] = "Already added.";
                return RedirectToAction("Books", "GuestViews", new { area = "Guest" });
            }

            return RedirectToAction("ShoppingCart");
        }

        public IActionResult Plus(int id)
        {
            var plus = _db.DbCart.Find(id);
            var check = _db.DbBooks.FirstOrDefault(x => x.Id == plus.BookId);

            if (plus.Quantity < check.Quantity)
            {
                plus.Quantity += 1;
                plus.Price = check.Price * plus.Quantity;
                _db.SaveChanges();
            }
            else
            {
                TempData["Info1"] = "No more books available";
            }

            return RedirectToAction("ShoppingCart");
        }

        public IActionResult Minus(int id)
        {
            var minus = _db.DbCart.Find(id);
            var check = _db.DbBooks.FirstOrDefault(x => x.Id == minus.BookId);

            minus.Quantity -= 1;

            if (minus.Quantity == 0)
            {
                _db.DbCart.Remove(minus);
            }
            else
            {
                minus.Price = minus.Price - check.Price;
            }

            _db.SaveChanges();
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult Delete(int id)
        {
            var todelete = _db.DbCart.Find(id);

            _db.DbCart.Remove(todelete);
            _db.SaveChanges();
            return RedirectToAction("ShoppingCart");
        }

        public async Task<IActionResult> Order()
        {
            var userclaim = (ClaimsIdentity)User.Identity;
            var userId = userclaim.FindFirst(ClaimTypes.NameIdentifier);

            var user = await _userManager.GetUserAsync(User);

            if (user.Name == null || user.Location == null || user.Address == null || user.PhoneNr == null || user.CreditCard == null)
            {
                TempData["Info"] = "You must edit your info first.";
                return LocalRedirect("/Identity/Account/Manage/EditInfo");
            }
            else
            {
                cartList2 = _db.DbCart.Where(x => x.TheUserId == userId.Value).ToList();

                if (cartList2.Count == 0)
                {
                    TempData["Info1"] = "Your order is empty";
                    return RedirectToAction("ShoppingCart");
                }

                foreach (var orderedItem in cartList2)
                {
                    var manage = _db.DbBooks.FirstOrDefault(x => x.Id == orderedItem.BookId);
                    var check = manage.Quantity - orderedItem.Quantity;

                    if (check < 0)
                    {
                        TempData["Info2"] = "Some books must have run out of stock.";
                        return RedirectToAction("ShoppingCart");
                    }
                    else
                    {
                        UserHistory stored = new UserHistory()
                        {
                            TheUserId = orderedItem.TheUserId,
                            Title = orderedItem.Title,
                            Author = orderedItem.Author,
                            Quantity = orderedItem.Quantity,
                            Price = orderedItem.Price,
                            OrderTime = DateTime.Now,
                            Name = user.Name
                        };
                        _db.DbHistory.Add(stored);
                        _db.DbCart.Remove(orderedItem);

                        manage.Quantity = manage.Quantity - orderedItem.Quantity;

                        _db.SaveChanges();
                    }
                }

                TempData["Info"] = "Your order was successful!";
                return RedirectToAction("History");
            }
        }
    }
}
