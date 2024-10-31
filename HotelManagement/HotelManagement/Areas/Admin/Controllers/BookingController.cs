using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Controllers
{
    [Area("Admin")]
    [Route("Admin/Booking")]
    public class BookingController : Controller
    {
        private readonly HotelDbContext db;
        public BookingController(HotelDbContext database)
        {
            db = database;
        }

        private string GenerateBookingID()
        {
            string prefix = "BK";
            var lastBooking = db.Bookings.OrderByDescending(x=>x.BookingID).FirstOrDefault();
            if(lastBooking == null)
            {
                return prefix + "00001";
            }
            else
            {
                var lastNumber = int.Parse(lastBooking.BookingID.Substring(prefix.Length));
                var newNumber = lastNumber + 1;
                return $"{prefix}{newNumber:D4}";
            }
        }
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var newBoookingID = GenerateBookingID();
            ViewBag.NewBookingID = newBoookingID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                var existingBooking = db.Bookings.FirstOrDefault(b => b.BookingID == booking.BookingID);
                if (existingBooking == null)
                {
                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("BookingID", "Booking ID already exists.");
                }
            }
            var newBoookingID = GenerateBookingID();
            ViewBag.NewBookingID = newBoookingID;
            return View(booking);
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index(string SortColumn = "BookingID", string iconClass = "fa-sort-asc", int page = 1, string searchQuery = "")
        {
            var booking = db.Bookings.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                booking = booking.Where(x => x.BookingID.Contains(searchQuery) ||
                                             x.CustomerID.Contains(searchQuery));
            }

            ViewBag.SearchQuery = searchQuery;
            ViewBag.SortColumn = SortColumn;
            ViewBag.IconClass = iconClass;
            booking = SortBooking(booking, SortColumn, iconClass);

            int NoOfRecoredPerPage = 5;
            int NoOfPages = (int)Math.Ceiling((double)booking.Count() / NoOfRecoredPerPage);
            int NoOfRecordToSkip = (page - 1) * NoOfRecoredPerPage;
            ViewBag.Page = page;
            ViewBag.NoOfPages = NoOfPages;
            booking = booking.Skip(NoOfRecordToSkip).Take(NoOfRecoredPerPage);

            return View(booking.ToList());
        }

        private IQueryable<Booking> SortBooking(IQueryable<Booking> booking, string SortColumn, string IconClass)
        {
            switch (SortColumn)
            {
                case "BookingID":
                    booking = IconClass == "fa-sort-asc" ? booking.OrderBy(x => x.BookingID) : booking.OrderByDescending(x => x.BookingID);
                    break;
                case "DateCome":
                    booking = IconClass == "fa-sort-asc" ? booking.OrderBy(x => x.DateCome) : booking.OrderByDescending(x => x.DateCome);
                    break;
                case "DateGo":
                    booking = IconClass == "fa-sort-asc" ? booking.OrderBy(x => x.DateGo) : booking.OrderByDescending(x => x.DateGo);
                    break;
                case "Status":
                    booking = IconClass == "fa-sort-asc" ? booking.OrderBy(x => x.Status) : booking.OrderByDescending(x => x.Status);
                    break;
                case "Deposit":
                    booking = IconClass == "fa-sort-asc" ? booking.OrderBy(x => x.Deposit) : booking.OrderByDescending(x => x.Deposit);
                    break;
                case "NumberPeople":
                    booking = IconClass == "fa-sort-asc" ? booking.OrderBy(x => x.NumberPeople) : booking.OrderByDescending(x => x.NumberPeople);
                    break;
            }
            return booking;
        }
        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            if(id == null || db.Bookings == null)
            {
                return NotFound();
            }
            var booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Booking booking)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(booking);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!BookingExists(booking.BookingID))
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
            return View(booking);
        }

        private bool BookingExists(string id) 
        {
            return(db.Bookings?.Any(x => x.BookingID == id)).GetValueOrDefault();
        }
        // Delete
        [Route("Delete")]
        public IActionResult Delete(string id)
        {
            //if(id == null || db.Bookings == null)
            //{
            //    return NotFound();
            //}
            //var booking = db.Bookings.Find(id);

            //if(booking == null)
            //{
            //    return NotFound();
            //}  
            //if(booking.BookingID.Count() >0)
            //{
            //    return Content("Can not delete this rooms");    
            //}    
            //return View(booking);

            //var booking = db.Bookings.Include(x => x.BookingDetail).FirstOrDefault(b => b.BookingID == id);
            //if(booking == null)
            //{
            //    return NotFound();
            //}
            //return View(booking);
        }
        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
      
        public IActionResult DeleteConfirmed(string id)
        {
            //if(db.Bookings == null)
            //{
            //    return Problem("Entity set 'Bookings' is null");
            //}    
            //var booking = db.Bookings?.Find(id);
            //if (booking != null)
            //{
            //    db.Bookings.Remove(booking);
            //}
            //db.SaveChanges();
            //return RedirectToAction(nameof(Index));
            //var booking = db.Bookings.Include(x => x.BookingDetail).FirstOrDefault(b => b.BookingID == id);
            //if (booking == null)
            //{
            //    return NotFound();
            //}
            //db.BookingDetails.RemoveRange(booking.BookingDetail);
            //db.Bookings.Remove(booking);
            //db.SaveChanges();
            //return RedirectToAction(nameof(Index));
        }
    }
}
