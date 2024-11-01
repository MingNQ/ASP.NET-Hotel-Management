using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Controllers
{
    [Route("/Room")]
    public class RoomController : Controller
    {
        private HotelDbContext db;

        public RoomController(HotelDbContext db)
        {
            this.db = db;
        }
        [Route("")]
        [Route("Index")]
        public IActionResult Index(int page = 1)
        {
          

            //ViewBag.Categories = db.Categories.Include(c => c.Rooms).ToList();
            ViewBag.Services = db.Services.ToList();

            int NoOfRecordPerPage = 5;
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

            return View(room);
        }

        [HttpGet]
        public IActionResult RoomAvailable(string id, DateTime dateCome, DateTime dateGo)
        {
            if (dateCome >= dateGo)
            {
                ModelState.AddModelError("", "Ngày đến phải nhỏ hơn ngày đi.");
                return View();
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

            ViewBag.Rooms = availableRooms;
            return View();
        }
    }
}
