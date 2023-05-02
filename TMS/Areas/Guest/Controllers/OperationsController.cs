using Microsoft.AspNetCore.Mvc;
using TMS.Models;
using TMS.Data;
using Newtonsoft.Json;

namespace TMS.Areas.Guest.Controllers
{
    [Area("Guest")]

    public class OperationsController : Controller
    {
        public List<BooksDB> booksList2 = new List<BooksDB>();

        // DB 
        private readonly TMSDbContext _db;

        public OperationsController(TMSDbContext db)
        {
            _db = db;
        }

        //Actions Sort/Category
        public IActionResult Sort(string sorting)
        {
            switch (sorting)
            {
                case "Name A-Z":
                    return RedirectToAction("BooksMenu", new { sorter = 1 });
                    break;
                case "Name Z-A":
                    return RedirectToAction("BooksMenu", new { sorter = 2 });
                    break;
                case "Price <":
                    return RedirectToAction("BooksMenu", new { sorter = 3 });
                    break;
                case "Price >":
                    return RedirectToAction("BooksMenu", new { sorter = 4 });
                    break;
                default:
                    return RedirectToAction("BooksMenu");
                    break;
            }
        }

        public IActionResult Category(string categorizing)
        {
            switch (categorizing)
            {
                case "Fantasy":
                    return RedirectToAction("BooksMenu", new { genre = "Fantasy" });
                    break;
                case "Adventure":
                    return RedirectToAction("BooksMenu", new { genre = "Adventure" });
                    break;
                case "Horror":
                    return RedirectToAction("BooksMenu", new { genre = "Horror" });
                    break;
                case "Drama":
                    return RedirectToAction("BooksMenu", new { genre = "Drama" });
                    break;
                case "Comedy":
                    return RedirectToAction("BooksMenu", new { genre = "Comedy" });
                    break;
                case "Dystopian":
                    return RedirectToAction("BooksMenu", new { genre = "Dystopian" });
                    break;
                case "Political":
                    return RedirectToAction("BooksMenu", new { genre = "Political" });
                    break;
                case "Science Fiction":
                    return RedirectToAction("BooksMenu", new { genre = "Science Fiction" });
                    break;
                case "History":
                    return RedirectToAction("BooksMenu", new { genre = "History" });
                    break;
                case "Romance":
                    return RedirectToAction("BooksMenu", new { genre = "Romance" });
                    break;
                default:
                    return RedirectToAction("BooksMenu");
                    break;
            }
        }

        public IActionResult BooksMenu(int sorter, string genre)
        {
            booksList2 = _db.DbBooks.Where(x => x.Genre1 == genre || x.Genre2 == genre).ToList();

            if (sorter == 1)
            {
                booksList2 = _db.DbBooks.OrderBy(x => x.Title).ToList();
            }
            else if (sorter == 2)
            {
                booksList2 = _db.DbBooks.OrderByDescending(x => x.Title).ToList();
            }
            else if (sorter == 3)
            {
                booksList2 = _db.DbBooks.OrderBy(x => x.Price).ToList();
            }
            else if (sorter == 4)
            {
                booksList2 = _db.DbBooks.OrderByDescending(x => x.Price).ToList();
            }

            return RedirectToAction("Books", "GuestViews", new { menu = true, command = JsonConvert.SerializeObject(booksList2.ToList()) });
        }
    }
}
