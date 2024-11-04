using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Controllers
{
    public class RoomController : Controller
    {
        private HotelDbContext db;

        public RoomController(HotelDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(int page = 1)
        {
          

            //ViewBag.Categories = db.Categories.Include(c => c.Rooms).ToList();
            ViewBag.Services = db.Services.ToList();

            int NoOfRecordPerPage = 6;
            int totalRoom = db.Categories.Count();
            int NoOfPages = (int)Math.Ceiling((double)totalRoom/ NoOfRecordPerPage);
            int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;

            ViewBag.Page = page;
            ViewBag.NoOfPages = NoOfPages;

            var categories = db.Categories.Include(c => c.Rooms)
                                          .Skip(NoOfRecordToSkip)
                                          .Take(NoOfRecordPerPage)
                                          .ToList();
            ViewBag.Categories = categories;
            return View();
        }
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = db.Categories.Where(c => c.CategoryID == id).Include(c => c.Rooms).FirstOrDefault();
            ViewBag.Services = db.Services.ToList();
            ViewBag.Rates = db.Rates.ToList();
            return View(room);
        }

        [HttpGet]
        public IActionResult RoomAvailable(string id, DateTime dateCome, DateTime dateGo, int numPeople)
        {
            if (dateCome >= dateGo)
            {
                TempData["Error"] = "Date Check-in/ Check-out is not valid!";

                return RedirectToAction(nameof(Index), "Home");
            }

            var unavailableRoomIds = db.RentForms
                .Where(r => (dateCome < r.DateCheckOut && dateGo > r.DateCheckIn))
                .Select(r => r.RoomID)
                .ToList();

            var availableRooms = db.Rooms
                .Where(r => !unavailableRoomIds.Contains(r.RoomID) && r.Status == "Vacant")
                .Include(r => r.Category)
                .Include(r => r.Images)
                .ToList();

            if (!string.IsNullOrEmpty(id))
            {
                availableRooms = availableRooms.Where(r => r.CategoryID == id).ToList();
            }
            ViewBag.DateCome = dateCome;
            ViewBag.DateGo = dateGo;
            ViewBag.Rooms = availableRooms;
            ViewBag.CategoryID = id;
            ViewBag.NumPeople = numPeople;

            return View();
        }
    }
}
