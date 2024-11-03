using HotelManagement.Areas.Admin.Common;
using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;


namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Staff")]
    public class StaffController : Controller
    {
        private HotelDbContext db;
        public StaffController(HotelDbContext db1)
        {
            db = db1;
        }


        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult Index(string searchPhone)
        {
            var staff = db.Staffs.AsQueryable();

            if (!string.IsNullOrEmpty(searchPhone))
            {
                staff = staff.Where(c => c.Phone.Contains(searchPhone));
            }

            var staffList = staff.ToList();

            // Kiểm tra nếu không có khách hàng nào thỏa mãn điều kiện tìm kiếm
            if (!staffList.Any() && !string.IsNullOrEmpty(searchPhone))
            {
                ViewData["NoResultsMessage"] = $"No customer found with phone number: {searchPhone}";
            }
            TempData["Message"] = "This is Staff Account!";
            ViewData["searchPhone"] = searchPhone;
            return View(staffList);
        }

        public IActionResult Index()
        {
            var lst = db.Staffs.Include(m => m.Account).ToList();
            return View(lst);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            string staffID = "";
            while (true)
            {
                int numID = (int)new Random().NextInt64(10000);
                staffID = "S" + numID.ToString("D4");

                if (!StaffExists(staffID))
                {  break; }
            }    
            ViewBag.staffID = staffID;

            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("StaffID,FirstName,LastName,Gender,Email,Phone,Address,Role")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                string fullName = staff.FirstName + " " + staff.LastName;
                
                while (true)
                {
                    Account account = Generate.GenerateAccount(fullName, staff.Role != Role.Manager ? AccountType.Staff : AccountType.Admin);

                    if (!db.Accounts.Any(a => a.Username == account.Username))
                    {
                        db.Accounts.Add(account);
                        staff.Account = account;
                        break;
                    }
                }

                db.Staffs.Add(staff);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StaffID = staff.StaffID;
            return View();
        }

        [Route("Edit")]
        public IActionResult Edit(string idStaff)
        {
            if (idStaff == null || db.Staffs == null)
            {
                return NotFound();
            }
            var staff = db.Staffs.Find(idStaff);
            if (staff == null)
            {
                return NotFound();
            }
			ViewBag.StaffID = staff.StaffID;
			return View(staff);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string idStaff, [Bind("StaffID,FirstName,LastName,Gender,Email,Phone,Address,Role")] Staff staff)
        {
            if (idStaff
                != staff.StaffID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(staff);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.StaffID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
			ViewBag.StaffID = staff.StaffID;
			return View(staff);
        }

        private bool StaffExists(string idStaff)
        {
            return (db.Staffs?.Any(e => e.StaffID == idStaff)) ?? false;
        }


        [Route("Delete")]
        public IActionResult Delete(string idStaff)
        {
            if (idStaff == null || db.Staffs == null)
            {
                return NotFound();
            }
            var staff = db.Staffs.FirstOrDefault(e => e.StaffID == idStaff);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        [HttpPost, ActionName("Delete")]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string idStaff)
        {
            var staff = db.Staffs.Find(idStaff);
            if (staff != null)
            {
                db.Staffs.Remove(staff);
            }
            db.SaveChanges();
            if (db.Invoices.Any(i => i.StaffID == idStaff) || db.RentForms.Any(r => r.StaffID == idStaff))
            {
                return Content("Error! Can't delete staff!");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

   