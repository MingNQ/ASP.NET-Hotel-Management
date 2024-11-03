using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Controllers
{
    public class HomeBookingController : Controller
    {
        private HotelDbContext db;

        public HomeBookingController(HotelDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(string categoryId, string roomId, DateTime dateCome, DateTime dateGo, int numPeople)
        {
            if (categoryId != null)
            {
                var category = db.Categories.Where(c => c.CategoryID == categoryId).Include(c => c.Rooms).FirstOrDefault();
				ViewBag.Rooms = new SelectList(category.Rooms.Where(r => r.Status == "Vacant").ToList(), "RoomID", "RoomID");
				ViewBag.Category = db.Categories.Where(c => c.CategoryID == categoryId).FirstOrDefault();
            }
            if (roomId != null)
            {
                var room = db.Rooms.Where(r => r.RoomID == roomId).Include(r => r.Category).FirstOrDefault();
                ViewBag.Category = room.Category;
				ViewBag.Room = room;
			}

			ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryID", "TypeName");
            
            if (dateCome < DateTime.Now && dateGo < DateTime.Now)
            {
                dateCome = DateTime.Now;
                dateGo = DateTime.Now;
            }

            ViewBag.DateCome = dateCome.ToString("MM/dd/yyyy");
            ViewBag.DateGo = dateGo.ToString("MM/dd/yyyy");
            ViewBag.NumPeople = numPeople;

			return View();
        }

        [HttpPost]
        public IActionResult Book(string rid, BookingViewModel model)
		{
            var rooms = db.Rooms.Include(r => r.Category).ToList();
            Room room = null;
			if (rid != null)
			{
				room = rooms.FirstOrDefault(r => r.RoomID == rid);
				ViewBag.Category = room.Category;
				ViewBag.Room = room;
			}

			// Generate Customer ID
			string customerID = "";
			string bookingID = "";
            string rentFormID = GenerateID("RF");

			while (true)
			{
				customerID = GenerateID("CUS");

				if (!db.Customers.Any(c => c.CustomerID == customerID))
				{
					model.Customer.CustomerID = customerID;
					break;
				}

			}

			while (true)
			{
				bookingID = GenerateID("BK");

				if (!db.Bookings.Any(b => b.BookingID == bookingID))
				{
					model.Booking.BookingID = bookingID;

                    // Remove state 
                    ModelState.Remove("Booking.BookingID");
					break;
				}
			}

            // Check valid 
			if (ModelState.IsValid)
            {
                try
                {
					db.Add(new Booking
					{
						BookingID = bookingID,
						CustomerID = customerID,
						DateCome = model.Booking.DateCome,
						DateGo = model.Booking.DateGo,
						NumberPeople = model.Booking.NumberPeople,
					});
					db.Add(new BookingDetail
                    {
                        BookingID = bookingID,
                        CategoryID = room.CategoryID,
                        NumberRoom = 1
                    });
                    db.Add(new RentForm
                    {
                        RentFormID = rentFormID,
                        BookingID = bookingID,
                        StaffID = "S0001",
                        RoomID = rid,
                        CustomerID = customerID,
                        DateCreate = DateTime.Now,
                        DateCheckIn = model.Booking.DateCome,
                        DateCheckOut = model.Booking.DateGo,
                        Sale = 0
                    });

                    db.Customers.Add(model.Customer);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }

            ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryID", "TypeName");

            return View("Index");
        }

        /// <summary>
        /// Generate Random ID
        /// </summary>
        /// <param name="idBase"></param>
        /// <returns></returns>
        private string GenerateID(string idBase)
        {
            string id = "";
            switch (idBase)
            {
                case "CUS":
					id = idBase + new Random().NextInt64(100000).ToString("d5");
                    break;
                case "BK":
					id = idBase + new Random().NextInt64(100000).ToString("d5");
                    break;
                case "RF":
                    id = idBase + new Random().NextInt64(100000).ToString("d5");
                    break;
            }

            return id;
        }
	}
}
