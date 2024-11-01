﻿using HotelManagement.Data;
using System.Linq;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Invoice")]
    public class InvoiceController : Controller
    {
        private HotelDbContext db;

        public InvoiceController(HotelDbContext db)
        {
            this.db = db;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            var invoices = db.Invoices.Include(i => i.Booking).ThenInclude(b => b.Customer)
                                    .Include(i => i.Booking).ThenInclude(b => b.RentForm).ThenInclude(rf => rf.Room).ThenInclude(r => r.Category)
                                    .Include(i => i.Booking)
                                    .ThenInclude(b => b.RentForm)
                                    .ThenInclude(rf => rf.Room)
                                    .ThenInclude(r => r.RoomServices)
                                    .ThenInclude(rs => rs.Service) 
                                    .Include(i => i.Staff)
                                    .Select(i => new
                                    {
                                        i.InvoiceID,
                                        i.BookingID,
                                        i.DateCreate,
                                        CustomerName = i.Booking.Customer.FirstName + " " + i.Booking.Customer.LastName,
                                        StaffName = i.Staff.FirstName + " " + i.Staff.LastName,

                                        // Thông tin tính tổng tiền
                                        DateCheckIn = i.Booking.RentForm.DateCheckIn,
                                        DateCheckOut = i.Booking.RentForm.DateCheckOut,
                                        CategoryPrice = i.Booking.RentForm.Room.Category.Price,
                                        Deposit = i.Booking.Deposit,
                                        Sale = i.Booking.RentForm.Sale,
                                        TotalServicePrice = i.Booking.RentForm.Room.RoomServices.Sum(rs => rs.Service.Price)
                                    })
                                    .ToList()
                                    .Select(i => new
                                    {
                                        i.InvoiceID,
                                        i.BookingID,
                                        i.DateCreate,
                                        i.CustomerName,
                                        i.StaffName,

                                        // Tính tổng tiền
                                        TotalMoney = ((decimal)(i.DateCheckOut - i.DateCheckIn).TotalDays *
                                                      (i.CategoryPrice + i.TotalServicePrice) * (1 - i.Sale) - i.Deposit)
                                    }).ToList();


            // Truyền dữ liệu cho ViewBag
            ViewBag.Invoices = invoices;
            return View(invoices);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var lastInvoiceID = db.Invoices.OrderByDescending(i => i.InvoiceID).Select(i => i.InvoiceID).FirstOrDefault();
            int newInvoiceNumber = lastInvoiceID != null? int.Parse(lastInvoiceID.Substring(3)) + 1: 1;
            string newInvoiceID = "INV" + newInvoiceNumber.ToString("D5");
            // Lấy danh sách BookingID
            var bookings = db.Bookings.Select(b => new { b.BookingID }).ToList();
            ViewBag.BookingIDs = bookings;
            // Lấy danh sách StaffID và StaffName
            var staffList = db.Staffs
                .Select(s => new
                {
                    s.StaffID,
                    StaffName = s.FirstName + " " + s.LastName
                })
                .ToList();
            ViewBag.StaffList = staffList;

            // Chuẩn bị dữ liệu cho form
            ViewBag.NewInvoiceID = newInvoiceID;
            ViewBag.DateCreate = DateTime.Now;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public IActionResult Create(string InvoiceID, DateTime DateCreate, string BookingID, string StaffID)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng Invoice mới
                var newInvoice = new Invoice
                {
                    InvoiceID = InvoiceID,
                    DateCreate = DateCreate,
                    BookingID = BookingID,
                    StaffID = StaffID
                };

                // Thêm hóa đơn mới vào cơ sở dữ liệu
                db.Invoices.Add(newInvoice);
                db.SaveChanges();

                // Chuyển hướng về trang Index sau khi tạo thành công
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, nạp lại form với thông tin đã nhập
            var bookings = db.Bookings.Select(b => new { b.BookingID }).ToList();
            ViewBag.BookingIDs = bookings;

            var staffList = db.Staffs
                .Select(s => new
                {
                    s.StaffID,
                    StaffName = s.FirstName + " " + s.LastName
                })
                .ToList();
            ViewBag.StaffList = staffList;

            ViewBag.NewInvoiceID = InvoiceID;
            ViewBag.DateCreate = DateCreate;

            return View();
        }

        [HttpGet]
        [Route("Details")]
        public IActionResult Details(string id)
        {
            
            var invoice = db.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(i => i.Staff)
                .Include(i => i.Booking.RentForm)
                    .ThenInclude(r => r.Room)
                        .ThenInclude(r => r.Category)
                .Include(i => i.Booking.RentForm.Room.RoomServices) 
                .ThenInclude(rs => rs.Service) 
                .FirstOrDefault(i => i.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound(); 
            }

            // Thông tin phòng đã thuê
            var rentedRooms = invoice.Booking.RentForm.Room;

            // Danh sách dịch vụ đã sử dụng
            var usedServices = rentedRooms.RoomServices;

            // Tính toán các thông tin cần thiết
            var dateCheckIn = invoice.Booking.RentForm.DateCheckIn;
            var dateCheckOut = invoice.Booking.RentForm.DateCheckOut;
            var numberOfDays = (decimal)(dateCheckOut - dateCheckIn).TotalDays;

            var categoryPrice = (decimal)rentedRooms.Category.Price;
            var totalServicePrice = (decimal)usedServices.Sum(s => s.Service.Price);
            var sale = (decimal)invoice.Booking.RentForm.Sale;
            var deposit = (decimal)invoice.Booking.Deposit;

            // Tính tổng tiền cần thanh toán
            var totalAmountToPay = (numberOfDays * (categoryPrice + totalServicePrice) * (1 - sale)) - deposit;

            // Truyền dữ liệu cho ViewBag
            ViewBag.Invoice = new
            {
                InvoiceID = invoice.InvoiceID,
                DateCreate = invoice.DateCreate,
                BookingID = invoice.BookingID,
                DateCheckIn = dateCheckIn,
                DateCheckOut = dateCheckOut,
                StaffName = $"{invoice.Staff.FirstName} {invoice.Staff.LastName}",
                CustomerName = $"{invoice.Booking.Customer.FirstName} {invoice.Booking.Customer.LastName}",
                RentedRooms = rentedRooms,
                UsedServices = usedServices,
                NumberOfDays = numberOfDays,
                CategoryPrice = categoryPrice,
                TotalServicePrice = totalServicePrice,
                Sale = sale,
                Deposit = deposit,
                TotalAmountToPay = totalAmountToPay
            };

            return View(); // Trả về view để hiển thị
        }




        [Route("Delete")]
        public IActionResult Delete(string id)
        {
            var invoice = db.Invoices.Include(i => i.Booking)
                                      .ThenInclude(b => b.Customer)
                                      .Include(i => i.Staff)
                                      .FirstOrDefault(i => i.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }
            var paymentExists = db.Payments.Any(p => p.InvoiceID == id);
            if (paymentExists)
            {
                return Content("Cannot delete this invoice because there are payment records associated with it.");
            }

            ViewBag.Invoice = invoice;
            return View(invoice);
        }

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return NotFound(); 
            }
            db.Invoices.Remove(invoice);
            db.SaveChanges();        
            return RedirectToAction("Index");
        }
    }
}