using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Controllers
{
    public class BookingController : Controller
    {
        private HotelDbContext db;

        public BookingController(HotelDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(string categoryId, string roomId)
        {
            if (categoryId != null)
                ViewBag.CategoryID = db.Categories.Where(c => c.CategoryID == categoryId).FirstOrDefault();
            if (roomId != null) 
                ViewBag.RoomID = db.Rooms.Where(r => r.RoomID == roomId).FirstOrDefault();

			ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryID", "TypeName");

			return View();
        }

        [HttpPost]
        public IActionResult Book(string rid, BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Generate Customer ID
                    string customerID = "";
                    string bookingID = "";

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
                            break;
                        }
                    }

                    db.Add(new Booking
                    {
                        BookingID = bookingID,
                        CustomerID = customerID,
                    });
                    db.Bookings.Add(model.Booking);
                    db.Customers.Add(model.Customer);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }

            if (model.BookingDetail.CategoryID != null)
                ViewBag.CategoryID = db.Categories.Where(c => c.CategoryID == model.BookingDetail.CategoryID).FirstOrDefault();
            if (rid != null)
                ViewBag.RoomID = db.Rooms.Where(r => r.RoomID == rid).FirstOrDefault();

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
			}

			return id;
        }
	}
}
