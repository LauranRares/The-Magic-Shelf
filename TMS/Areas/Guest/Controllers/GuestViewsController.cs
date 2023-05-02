using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMS.Data;
using TMS.Models;

namespace TMS.Areas.Guest.Controllers
{
    [Area("Guest")]

    public class GuestViewsController : Controller
    {
        public List<BooksDB> booksList = new List<BooksDB>();

        //DB
        private readonly TMSDbContext _db;

        public GuestViewsController(TMSDbContext db)
        {
            _db = db;
        }

        // Actions View
        public IActionResult Home()
        {
            return View();
        }

        public IActionResult Books(bool menu, string command)
        {
            if (menu)
            {
                booksList = JsonConvert.DeserializeObject<List<BooksDB>>(command);
            }
            else
            {
                booksList = _db.DbBooks.OrderBy(x => x.Title).ToList();
            }
            return View(booksList);
        }
    }
}
