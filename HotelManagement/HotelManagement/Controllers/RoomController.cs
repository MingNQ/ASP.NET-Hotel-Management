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

        public IActionResult Index()
        {
            ViewBag.Categories = db.Categories.Include(c => c.Rooms).ToList();
            ViewBag.Services = db.Services.ToList();

            return View();
        }

        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = db.Categories.Where(c => c.CategoryID == id).FirstOrDefault();
            ViewBag.Services = db.Services.ToList();

            return View(room);
        }
    }
}
