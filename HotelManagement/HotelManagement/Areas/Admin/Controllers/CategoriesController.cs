using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Areas.Admin.Controllers
{       
    [Area("Admin")]
    [Route("Admin/Categories")]
    public class CategoriesController : Controller
    {
        private HotelDbContext db;

        public CategoriesController(HotelDbContext db)
        {
            this.db = db;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index(string SortColumn = "CategoryID", string IconClass = "fa-sort-asc", int page = 1)
        {
            var categories = db.Categories.AsQueryable();
            // Sắp xếp
            ViewBag.SortColumn = SortColumn;
            ViewBag.IconClass = IconClass;
            categories = SortRooms(categories, SortColumn, IconClass);

            // Phân trang
            int NoOfRecordPerPage = 5;
            int NoOfPages = (int)Math.Ceiling((double)categories.Count() / NoOfRecordPerPage);
            int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;
            ViewBag.Page = page;
            ViewBag.NoOfPages = NoOfPages;
            categories = categories.Skip(NoOfRecordToSkip).Take(NoOfRecordPerPage);
            return View(categories.ToList());
        }
        // Phương thức sắp xếp riêng trả về IQueryable
        private IQueryable<Category> SortRooms(IQueryable<Category> categories, string SortColumn, string IconClass)
        {
            if (SortColumn == "CategoryID")
            {
                categories = IconClass == "fa-sort-asc" ? categories.OrderBy(r => r.CategoryID) : categories.OrderByDescending(r => r.CategoryID);
            }
            else if (SortColumn == "Price")
            {
                categories = IconClass == "fa-sort-asc" ? categories.OrderBy(r => r.Price) : categories.OrderByDescending(r => r.Price);
            }
            else if (SortColumn == "TypeName")
            {
                categories = IconClass == "fa-sort-asc" ? categories.OrderBy(r => r.TypeName) : categories.OrderByDescending(r => r.TypeName);
            }else if(SortColumn == "Capacity")
            {
                categories = IconClass == "fa-sort-asc" ? categories.OrderBy(r => r.Capacity) : categories.OrderByDescending(r => r.Capacity);
            }
            return categories;
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            // Check role
            var roleString = HttpContext.Session.GetString("Role");
            if (Enum.TryParse(roleString, out AccountType role) && role != AccountType.Admin)
            {
                // Not permission
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.TypeName = new List<string> { "Standard Single", "Standard Twin", "Superior Single", "Superior Twin", "Superior Triple", "Deluxe Single", "Deluxe Twin", "Suite Single", "Suite Twin", "Suite Triple", "Suite Queen"};
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                Category oldCategory = db.Categories.Find(category.CategoryID);
                if(oldCategory == null)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("CategoryID", "CategoryID đã tồn tại.");
                }
            }
            ViewBag.TypeName = new List<string> { "Standard Single", "Standard Twin", "Superior Single", "Superior Twin", "Superior Triple", "Deluxe Single", "Deluxe Twin", "Suite Single", "Suite Twin", "Suite Triple", "Suite Queen" };
            return View(category);
        }

        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            // Check role
            var roleString = HttpContext.Session.GetString("Role");
            if (Enum.TryParse(roleString, out AccountType role) && role != AccountType.Admin)
            {
                // Not permission
                return RedirectToAction("AccessDenied", "Account");
            }

            if (id == null || db.Categories == null)
            {
                return NotFound();
            }
            var category = db.Categories.Find(id);
            if(category == null)
            {
                return NotFound();
            }
            ViewBag.TypeName = new List<string> { "Standard Single", "Standard Twin", "Superior Single", "Superior Twin", "Superior Triple",
        "Deluxe Single", "Deluxe Twin", "Suite Single", "Suite Twin", "Suite Triple", "Suite Queen"};
            return View(category);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(category);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        private bool CategoryExists(string id)
        {
            return (db.Categories?.Any(e => e.CategoryID == id)).GetValueOrDefault();
        }

        [Route("Delete")]
        public IActionResult Delete(string id)
        {
            // Check role
            var roleString = HttpContext.Session.GetString("Role");
            if (Enum.TryParse(roleString, out AccountType role) && role != AccountType.Admin)
            {
                // Not permission
                return RedirectToAction("AccessDenied", "Account");
            }

            if (id== null || db.Categories == null)
            {
                return NotFound();
            }

            var category = db.Categories.Include(r => r.Rooms).FirstOrDefault(c => c.CategoryID == id);

            if(category == null)
            {
                return NotFound();
            }

            if(category.Rooms.Count() > 0)
            {
                return Content("This Category has some rooms, can't delete!");

            }
            return View(category);
        }

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (db.Categories == null)
            {
                return Problem("Entity set 'Categories' is null.");
            }
            var category = db.Categories.Find(id);
            if(category != null)
            {
                db.Categories.Remove(category);
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
