using Microsoft.AspNetCore.Mvc;
using TMS.Models;
using TMS.Data;
using Microsoft.AspNetCore.Authorization;
using TMS.Roles;

namespace TMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = TheRoles._Admin)]

    public class TMSController : Controller
    {
        public List<BooksDB> booksList = new List<BooksDB>();

        // DB + upload
        private readonly TMSDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public TMSController(TMSDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        // Actions View       
        public IActionResult Manage()
        {
            booksList = _db.DbBooks.OrderBy(x => x.Title).ToList();
            return View(booksList);
        }

        public IActionResult AddBook(BooksDB addagain)
        {
            return View(addagain);
        }

        public IActionResult Edit(int id)
        {
            var foredit = _db.DbBooks.Find(id);
            return View(foredit);
        }


        //Actions Interact/Redirect
        [ValidateAntiForgeryToken]

        public IActionResult Adding(BooksDB adding, IFormFile img)
        {
            if (ModelState.IsValid /*&& adding.Genre1 != adding.Genre2*/)
            {
                //Add image
                string wwwRoot = _hostEnvironment.WebRootPath;

                string imgName = Path.GetFileNameWithoutExtension(img.FileName);
                var extension = Path.GetExtension(img.FileName);
                var upload = Path.Combine(wwwRoot, @"Images");

                using (var imgStreams = new FileStream(Path.Combine(upload, imgName + extension), FileMode.Create))
                {
                    img.CopyTo(imgStreams);
                }

                adding.Image = @"\Images\" + imgName + extension;
                ///////////

                _db.DbBooks.Add(adding);
                _db.SaveChanges();
                return RedirectToAction("Manage");
            }
            else
            {
                TempData["Mistake"] = "Make sure all the data is correct";
                return RedirectToAction("AddBook", adding);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(BooksDB editing)
        {
            var check = ModelState["Price"];

            if (!check.Errors.Any()) // && editing.Genre1 != editing.Genre2)
            {
                _db.DbBooks.Update(editing);
                _db.SaveChanges();
                return RedirectToAction("Manage");
            }
            else
            {
                TempData["Mistake"] = "Make sure all the data is correct";
                return RedirectToAction("Edit");
            }
        }

        [ValidateAntiForgeryToken]

        public IActionResult EditImage(int id, IFormFile img)
        {
            var imgedit = _db.DbBooks.Find(id);

            if (img != null)
            {
                var oldimg = Path.Combine(_hostEnvironment.WebRootPath, imgedit.Image.TrimStart('\\'));
                if (System.IO.File.Exists(oldimg))
                {
                    System.IO.File.Delete(oldimg);
                }

                string imgName = Path.GetFileNameWithoutExtension(img.FileName);
                var extension = Path.GetExtension(img.FileName);
                var upload = Path.Combine(_hostEnvironment.WebRootPath, @"Images");

                using (var imgStreams = new FileStream(Path.Combine(upload, imgName + extension), FileMode.Create))
                {
                    img.CopyTo(imgStreams);
                }

                imgedit.Image = @"\Images\" + imgName + extension;

                TempData["ImageUpload"] = "The image was succesfully changed";

                _db.DbBooks.Update(imgedit);
                _db.SaveChanges();
            }
            else
            {
                TempData["NoImage"] = "There is no image uploaded";
            }

            return RedirectToAction("Edit", new { id = imgedit.Id });
        }


        public IActionResult Delete(int id)
        {
            var todelete = _db.DbBooks.Find(id);

            System.IO.File.Delete(Path.Combine(_hostEnvironment.WebRootPath, todelete.Image.TrimStart('\\')));

            _db.DbBooks.Remove(todelete);
            _db.SaveChanges();
            return RedirectToAction("Manage");
        }
    }
}
