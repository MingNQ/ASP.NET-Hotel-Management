using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/CheckRoomVacant")]
    public class CheckRoomVacantController : Controller
    {
        private readonly HotelDbContext db;
        private const int PageSize = 5;

        public CheckRoomVacantController(HotelDbContext db)
        {
            this.db = db;
        }

        [Route("")]
        [Route("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("CheckAvailability")]
        [HttpGet]
        public async Task<IActionResult> CheckAvailability(DateTime dateCome, DateTime dateGo, int page = 1, string sortColumn = "RoomID", string sortDirection = "asc")
        {
            if (dateCome >= dateGo)
            {
                ModelState.AddModelError("", "Ngày đến phải nhỏ hơn ngày đi.");
                return View();
            }

            
            var unavailableRoomIds = await db.RentForms
                .Where(r => (dateCome < r.DateCheckOut && dateGo > r.DateCheckIn))
                .Select(r => r.RoomID)
                .ToListAsync();

            // Lọc danh sách phòng trống
            var availableRoomsQuery = db.Rooms
                .Where(r => !unavailableRoomIds.Contains(r.RoomID) && r.Status == "Vacant")
                .Include(r => r.Category)
                .AsQueryable();

            // Sắp xếp danh sách phòng
            availableRoomsQuery = sortColumn switch
            {
                "RoomID" => sortDirection == "asc" ? availableRoomsQuery.OrderBy(r => r.RoomID) : availableRoomsQuery.OrderByDescending(r => r.RoomID),
                "Category" => sortDirection == "asc" ? availableRoomsQuery.OrderBy(r => r.Category.TypeName) : availableRoomsQuery.OrderByDescending(r => r.Category.TypeName),
                _ => availableRoomsQuery.OrderBy(r => r.RoomID),
            };

            // Tổng số phòng
            int totalRooms = await availableRoomsQuery.CountAsync();

            // Phân trang
            var availableRooms = await availableRoomsQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Truyền thông tin phân trang và sắp xếp vào View
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRooms / PageSize);
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.DateCome = dateCome;
            ViewBag.DateGo = dateGo;

            return View(availableRooms);
        }
    }
}
