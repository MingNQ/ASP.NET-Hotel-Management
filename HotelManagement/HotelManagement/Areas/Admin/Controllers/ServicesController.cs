using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Areas.Admin.Controllers
{       
    [Area("Admin")]  
    [Route("Admin/Services")]
    public class ServicesController : Controller
    {
        private HotelDbContext db;

        public ServicesController(HotelDbContext db)
        {
            this.db = db;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index(string SortColumn = "CategoryID", string IconClass = "fa-sort-asc")
        {
            var services = db.Services.AsQueryable();
            // Sắp xếp
            ViewBag.SortColumn = SortColumn;
            ViewBag.IconClass = IconClass;
            services = SortRooms(services, SortColumn, IconClass);
            return View(services.ToList());
        }

        private IQueryable<Service> SortRooms(IQueryable<Service> services, string SortColumn, string IconClass)
        {
            if (SortColumn == "ServiceID")
            {
                services = IconClass == "fa-sort-asc" ? services.OrderBy(r => r.ServiceID) : services.OrderByDescending(r => r.ServiceID);
            }
            else if (SortColumn == "Price")
            {
                services = IconClass == "fa-sort-asc" ? services.OrderBy(r => r.Price) : services.OrderByDescending(r => r.Price);
            }
            else if (SortColumn == "ServiceName")
            {
                services = IconClass == "fa-sort-asc" ? services.OrderBy(r => r.ServiceName) : services.OrderByDescending(r => r.ServiceName);
            }
            return services;
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var lastService = db.Services.OrderByDescending(s => s.ServiceID).FirstOrDefault();
            string newServiceID = "R0001";
            if (lastService != null)
            {
                int lastNumber = int.Parse(lastService.ServiceID.Substring(1)) + 1;
                newServiceID = "R" + lastNumber.ToString("D4");
            }
            ViewBag.NewServiceID = newServiceID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public IActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {

                db.Services.Add(service);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            if (id == null || db.Services == null)
            {
                return NotFound();
            }
            var service = db.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Service service)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(service);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ServiceID))
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
            return View(service);
        }
        private bool ServiceExists(string id)
        {
            return (db.Services?.Any(e => e.ServiceID == id)).GetValueOrDefault();
        }

        [Route("Delete")]
        public IActionResult Delete(string id)
        {
            if (id == null || db.Categories == null)
            {
                return NotFound();
            }

            var service = db.Services.Include(r => r.RoomServices).FirstOrDefault(c => c.ServiceID == id);

            if (service == null)
            {
                return NotFound();
            }

            if (service.RoomServices.Count() > 0)
            {
                return Content("This Service has some rooms, can't delete!");

            }
            return View(service);
        }

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (db.Services == null)
            {
                return Problem("Entity set 'Services' is null.");
            }
            var service = db.Services.Find(id);
            if (service != null)
            {
                db.Services.Remove(service);
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
