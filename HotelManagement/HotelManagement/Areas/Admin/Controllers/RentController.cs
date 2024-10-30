using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HotelManagement.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Rent")]
	public class RentController : Controller
	{
		private HotelDbContext db;

		public RentController(HotelDbContext context)
		{
			this.db = context;
		}

		[Route("")]
		[Route("Index")]
		public IActionResult Index()
		{
			var listRentForm = db.RentForms.Include(r => r.Staff).ToList();

			return View(listRentForm);
		}

		[Route("Details")]
		public IActionResult Details(string id)
		{
            if (id == null)
            {
                return NotFound();
            }
			var rentForm = db.RentForms.
								Include(r => r.Staff).
								Include(r => r.Room).
									ThenInclude(r => r.Category).
								Include(r => r.Room.RoomServices).
									ThenInclude(rs => rs.Service).
								FirstOrDefault(r => r.RentFormID == id);
            if (rentForm == null)
            {
                return NotFound();
            }

			ViewBag.RoomServices = rentForm.Room.RoomServices;

            return View(rentForm);
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			var rooms = db.Rooms.Where(r => r.Status == "Vacant").ToList();
			ViewBag.RoomVacant = new SelectList(rooms, "RoomID", "RoomID");

			return View();
		}


		[HttpPost]
		[Route("Create")]
		[ValidateAntiForgeryToken]
		public IActionResult Create([Bind("BookingID, RoomID, DateCreate, DateCheckIn, DateCheckOut, Sale")]RentForm rent)
		{
			return View();
		}

        [HttpGet]
		[Route("Edit")]
        public IActionResult Edit(string id)
        {
			if (id == null)
			{
				return NotFound();
			}

			var rentForm = db.RentForms.Where(r => r.RentFormID  == id).FirstOrDefault();

			if (rentForm == null)
			{
                return NotFound();
            }

            var rooms = db.Rooms.Where(r => r.Status == "Vacant").ToList();
            ViewBag.RoomVacant = new SelectList(rooms, "RoomID", "RoomID");
            return View(rentForm);
        }


        [HttpPost]
		[Route("Edit")]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(string rentFormID, [Bind("BookingID, RoomID, DateCreate, DateCheckIn, DateCheckOut, Sale")] RentForm rent)
        {
            return View();
        }

		[HttpGet]
		[Route("Delete")]
		public IActionResult Delete(string rentFormID)
		{
			return View();
		}

		[HttpPost]
		[Route("Delete")]
		public IActionResult DeleteConfirm(string rentFormID)
		{
			return View();
		}
    }
}
