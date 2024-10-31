using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Models.Common;
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
			var listRentForm = db.RentForms.
                            Include(r => r.Customer).
                            Include(r => r.Staff).
                            OrderByDescending(r => r.DateCreate).
                            ToList();

			return View(listRentForm);
		}

		[Route("Details")]
		public IActionResult Details(string id)
		{
            if (id == null)
            {
                return NotFound();
            }

            // Get information for Rent Form
            var rentForm = db.RentForms.
								Include(r => r.Customer).
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
			var rooms = db.Rooms.Where(r => r.Status == "Vacant").
                                Include(r => r.Category).
                                ToList();
            var services = db.Services.ToList();
            var customers = db.Customers.Select(c => new
            {
                CustomerID = c.CustomerID,
                FullName = c.FirstName + " " + c.LastName,
                Membership = c.Membership
            }).ToList();


			// Generate RentForm ID
			string rentFormID = "RF000001";

			while (true)
			{
				int numberID = (int)new Random().NextInt64(1000000);
				rentFormID = "RF" + numberID.ToString("D5");

				if (!ExistRentform(rentFormID))
					break;
			}

			// Get information for Rent Form
			ViewBag.Categories = new SelectList(rooms.Select(r => new { r.CategoryID, r.Category?.TypeName }).ToList(), "CategoryID", "TypeName");
			ViewBag.BookingList = new SelectList(db.Bookings.ToList(), "BookingID", "BookingID");
            ViewBag.Staffs = new SelectList(db.Staffs.Select(s => new
            {
                StaffID = s.StaffID,
                FullName = s.FirstName + " " + s.LastName
            }).ToList(), "StaffID", "FullName");
            ViewBag.Customers = new SelectList(customers, "CustomerID", "FullName");
            ViewBag.RoomVacant = new SelectList(rooms, "RoomID", "RoomID");
			ViewBag.RentFormID = rentFormID;
            ViewBag.Services = services;
            ViewBag.CustomerMemberShip = customers.ToDictionary(c => c.CustomerID, c => c.Membership);

			return View();
		}

		[HttpPost]
		[Route("Create")]
		[ValidateAntiForgeryToken]
		public IActionResult Create([Bind("RentFormID, BookingID, RoomID, StaffID, CustomerID, DateCreate, DateCheckIn, DateCheckOut, Sale")]RentForm rent, string[] SelectedServices)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            var rooms = db.Rooms.Where(r => r.Status == "Vacant").
								Include(r => r.Category).
								ToList();

			// Get information for Rent Form
			ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryID", "TypeName");
            ViewBag.BookingList = new SelectList(db.Bookings.ToList(), "BookingID", "BookingID");
			ViewBag.Staffs = new SelectList(db.Staffs.Select(s => new
			{
				StaffID = s.StaffID,
				FullName = s.FirstName + " " + s.LastName
			}).ToList(), "StaffID", "FullName");
			ViewBag.Customers = new SelectList(db.Customers.Select(c => new
			{
				CustomerID = c.CustomerID,
				FullName = c.FirstName + " " + c.LastName
			}).ToList(), "CustomerID", "FullName");
			ViewBag.RoomVacant = new SelectList(rooms, "RoomID", "RoomID");
            ViewBag.RentFormID = rent.RentFormID;
            ViewBag.Services = db.Services.ToList();

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

			// Get information for Rent Form
			var rentForm = db.RentForms.Where(r => r.RentFormID  == id).Include(r => r.Staff).Include(r => r.Customer).FirstOrDefault();

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
		public IActionResult Edit(string id, [Bind("RentFormID, RoomID, StaffID, CustomerID, DateCreate, DateCheckIn, DateCheckOut, Sale")] RentForm rent)
		{
            if (id != rent.RentFormID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
				try
				{
					var existingRent = db.RentForms.Find(id);
					if (existingRent == null)
					{
						return NotFound();
					}
					if (rent.DateCheckOut.ToUniversalTime() <= DateTime.UtcNow)
					{
						var room = db.Rooms.FirstOrDefault(r => r.RoomID == existingRent.RoomID);
						if (room != null && room.Status == "Occupied")
						{
							room.Status = "Vacant";
							db.Entry(room).State = EntityState.Modified;
						}
					}
					else
					{
						// Change Status Room
						if (existingRent.RoomID != rent.RoomID)
						{
							var oldRoom = db.Rooms.FirstOrDefault(r => r.RoomID == existingRent.RoomID);

							// Update Status 
							if (oldRoom != null)
							{
								oldRoom.Status = "Vacant";
								db.Entry(oldRoom).State = EntityState.Modified;
							}

							var newRoom = db.Rooms.FirstOrDefault(r => r.RoomID == rent.RoomID);
							if (newRoom != null)
							{
								newRoom.Status = "Occupied";
								db.Entry(newRoom).State = EntityState.Modified;
							}
						}
						else
						{
							// Update Status Room if wrong
							var room = db.Rooms.FirstOrDefault(r => r.RoomID == existingRent.RoomID);
							if (room != null && room.Status == "Vacant")
							{
								room.Status = "Occupied";
								db.Entry(room).State = EntityState.Modified;
							}
						}
					}

					// Update RentForm
					existingRent.RoomID = rent.RoomID;
                    existingRent.DateCheckIn = rent.DateCheckIn;
                    existingRent.DateCheckOut = rent.DateCheckOut;
                    existingRent.Sale = rent.Sale;

                    db.Entry(existingRent).Property(r => r.RoomID).IsModified = true;
                    db.Entry(existingRent).Property(r => r.DateCheckIn).IsModified = true;
                    db.Entry(existingRent).Property(r => r.DateCheckOut).IsModified = true;
                    db.Entry(existingRent).Property(r => r.Sale).IsModified = true;

                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExistRentform(rent.RentFormID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
			// If error load page again
            var rentForm = db.RentForms.Where(r => r.RentFormID == rent.RentFormID).Include(r => r.Staff).Include(r => r.Customer).FirstOrDefault();

            if (rentForm == null)
            {
                return NotFound();
            }

            var rooms = db.Rooms.Where(r => r.Status == "Vacant").ToList();
            ViewBag.RoomVacant = new SelectList(rooms, "RoomID", "RoomID");
            return View(rentForm);
        }

		[HttpGet]
		[Route("Delete")]
		public IActionResult Delete(string id)
		{
            if (id == null)
            {
                return NotFound();
            }

            // Get information for Rent Form
            var rentForm = db.RentForms.Where(r => r.RentFormID == id).Include(r => r.Staff).Include(r => r.Customer).FirstOrDefault();
            if (rentForm == null)
            {
                return NotFound();
            }

            return View(rentForm);
		}

		[HttpPost]
		[Route("Delete")]
		public IActionResult DeleteConfirm(string rentFormID)
		{
			return View();
		}

		/// <summary>
		/// Check If Exist Rent Form
		/// </summary>
		/// <param name="id">Rent Form ID</param>
		/// <returns></returns>
		private bool ExistRentform(string id)
		{
			return db.RentForms.Any(r => r.RoomID == id);
		}
    }
}
