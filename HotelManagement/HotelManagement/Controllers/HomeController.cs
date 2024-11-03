using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private HotelDbContext db;

        public HomeController(ILogger<HomeController> logger, HotelDbContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult Index()
        {
            var previewRoom = db.Categories.ToList();
            var rooms = new List<Category>();
            Dictionary<string, bool> map = new Dictionary<string, bool>();

            // Get list preview Room
            foreach (var room in previewRoom)
            {
                string currId = room.CategoryID.Substring(0, room.CategoryID.Length - 2);
                map[currId] = true;
            }

            foreach (var room in previewRoom)
            {
                string currId = room.CategoryID.Substring(0, room.CategoryID.Length - 2);
                
                if (map[currId] == true)
                {
                    rooms.Add(room);
                    map[currId] = false;
                }
            }

            ViewBag.Rates = db.Rates.ToList();
            ViewBag.Rooms = rooms;
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Blank()
        {
            return View("BlankPage");
        }
    }
}
