using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
   
    [Route("Admin/Rooms")]
    public class RoomsController : Controller
    {
        private HotelDbContext db;

        public RoomsController(HotelDbContext db)
        {
            this.db = db;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index(string SortColumn = "RoomID", string IconClass = "fa-sort-asc", int page = 1, string? CategoryID = null, string? Status = null)
        {
            var rooms = db.Rooms.Include(r => r.Category)
                                .Include(i => i.Images)
                                .AsQueryable();

            // Lưu giá trị của CategoryID và Status đã chọn vào ViewBag
            ViewBag.SelectedCategoryID = CategoryID;
            ViewBag.SelectedStatus = Status;

            // Lọc theo CategoryID và Status
            if (!string.IsNullOrEmpty(CategoryID))
            {
                rooms = rooms.Where(r => r.CategoryID == CategoryID);
            }

            if (!string.IsNullOrEmpty(Status))
            {
                rooms = rooms.Where(r => r.Status == Status);
            }

            // Sắp xếp
            ViewBag.SortColumn = SortColumn;
            ViewBag.IconClass = IconClass;
            rooms = SortRooms(rooms, SortColumn, IconClass);

            // Phân trang
            int NoOfRecordPerPage = 5;
            int NoOfPages = (int)Math.Ceiling((double)rooms.Count() / NoOfRecordPerPage);
            int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;
            ViewBag.Page = page;
            ViewBag.NoOfPages = NoOfPages;
            rooms = rooms.Skip(NoOfRecordToSkip).Take(NoOfRecordPerPage);

            // Lấy danh sách Categories cho dropdown
            ViewBag.Categories = db.Categories.ToList();

            return View(rooms.ToList());
        }


        // Phương thức sắp xếp riêng, trả về IQueryable
        private IQueryable<Room> SortRooms(IQueryable<Room> rooms, string SortColumn, string IconClass)
        {
            if (SortColumn == "RoomID")
            {
                rooms = IconClass == "fa-sort-asc" ? rooms.OrderBy(r => r.RoomID) : rooms.OrderByDescending(r => r.RoomID);
            }
            else if (SortColumn == "Price")
            {
                rooms = IconClass == "fa-sort-asc" ? rooms.OrderBy(r => r.Category.Price) : rooms.OrderByDescending(r => r.Category.Price);
            }
            else if (SortColumn == "CategoryID")
            {
                rooms = IconClass == "fa-sort-asc" ? rooms.OrderBy(r => r.CategoryID) : rooms.OrderByDescending(r => r.CategoryID);
            }
            return rooms;
        }



        [Route("Details")]
        public IActionResult Details(string id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var room = db.Rooms.Include(c => c.Category)
                       .Include(i => i.Images)
                       .Include(s => s.RoomServices)
                       .ThenInclude(rs => rs.Service) // Include Service for RoomService
                       .Include(f => f.RentForms)
                       .FirstOrDefault(r => r.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room); 
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            // Check role
            var roleString = HttpContext.Session.GetString("Role");
            if (Enum.TryParse(roleString, out AccountType role) && role != AccountType.Admin)
            {
                // Not permission
                return RedirectToAction("AccessDenied", "Account");
            }

            var lastRoom = db.Rooms.OrderByDescending(r => r.RoomID).FirstOrDefault();
            string newRoomID = "R001";
            if(lastRoom != null)
            {
                int lastNumber = int.Parse(lastRoom.RoomID.Substring(1)) + 1;
                newRoomID = "R" + lastNumber.ToString("D3");
            }
            ViewBag.NewRoomID = newRoomID; 
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName");
            ViewBag.Services = db.Services.ToList(); // Lấy danh sách dịch vụ
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public IActionResult Create([Bind("RoomID,CategoryID, Status")] Room room, List<IFormFile> RoomImages, List<string> selectedServices)
        {
            if (ModelState.IsValid)
            {
                db.Rooms.Add(room);
                db.SaveChanges();

                var lastImage = db.Images.OrderByDescending(i => i.ImageID).FirstOrDefault();
                int lastNumber = lastImage != null ? int.Parse(lastImage.ImageID.Substring(3)) : 0;

                foreach (var imageFile in RoomImages)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string newImageID = "IMG" + (lastNumber + 1).ToString("D4");
                        lastNumber++;

                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageFile.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }

                        var image = new Image
                        {
                            ImageID = newImageID,
                            RoomID = room.RoomID,
                            ImageUrl = "/img/" + imageFile.FileName
                        };

                        db.Images.Add(image);
                    }
                }

                // Lưu danh sách dịch vụ đã chọn
                foreach (var serviceId in selectedServices)
                {
                    var roomService = new RoomService
                    {
                        RoomID = room.RoomID,
                        ServiceID = serviceId
                    };
                    db.RoomServices.Add(roomService);
                }

                db.SaveChanges(); // Lưu tất cả thay đổi vào cơ sở dữ liệu

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName");
            ViewBag.Services = db.Services.ToList(); // Lấy lại danh sách dịch vụ nếu có lỗi
            return View(room);
        }


        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            // Check role
            var roleString = HttpContext.Session.GetString("Role");
            if (Enum.TryParse(roleString, out AccountType role) && role != AccountType.Admin)
            {
                // Not permission
                return RedirectToAction("AccessDenied", "Account");
            }

            if (id == null || db.Rooms == null)
            {
                return NotFound();
            }
            //var room = db.Rooms.Find(id);
            var room = db.Rooms.Include(c => c.Category)
                              .Include(i => i.Images)
                              .Include(s => s.RoomServices)
                              .ThenInclude(rs => rs.Service)// Include Service for RoomService
                              .Include(f => f.RentForms)
                              .FirstOrDefault(r => r.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName", room.CategoryID);
            ViewBag.Services = db.Services.ToList(); // Lấy danh sách tất cả dịch vụ
            ViewBag.SelectedServices = room.RoomServices.Select(rs => rs.ServiceID).ToList(); // Dịch vụ hiện tại của phòng

            return View(room);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("RoomID, CategoryID, Status")] Room room, List<IFormFile> RoomImages, List<string> selectedServices)
        {
            if (id != room.RoomID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm phòng hiện tại trong database và bao gồm danh sách hình ảnh, dịch vụ liên quan
                    var existingRoom = db.Rooms.Include(r => r.Images)
                                       .Include(r => r.RoomServices)
                                       .FirstOrDefault(r => r.RoomID == id);

                    if (existingRoom == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin phòng
                    existingRoom.CategoryID = room.CategoryID;
                    existingRoom.Status = room.Status;

                    if(RoomImages != null && RoomImages.Any())
                    {
                        // Thiết lập ImageID cuối cùng, bắt đầu từ 0 nếu không có ảnh nào trong hệ thống
                        var lastImage = db.Images.OrderByDescending(i => i.ImageID).FirstOrDefault();
                        int lastNumber = lastImage != null ? int.Parse(lastImage.ImageID.Substring(3)) : 0;

                        // Thêm mới hoặc cập nhật các ảnh
                        int imageIndex = 0;
                        foreach (var imageFile in RoomImages)
                        {
                            if (imageFile != null && imageFile.Length > 0)
                            {
                                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageFile.FileName);
                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    imageFile.CopyTo(stream);
                                }

                                if (imageIndex < existingRoom.Images.Count)
                                {
                                    // Cập nhật ImageUrl cho ảnh hiện có
                                    var existingImage = existingRoom.Images.ElementAt(imageIndex);
                                    existingImage.ImageUrl = "/img/" + imageFile.FileName;
                                }
                                else
                                {
                                    // Tạo mới ImageID và thêm ảnh vào database
                                    lastNumber++;
                                    string newImageID = "IMG" + lastNumber.ToString("D4");

                                    var newImage = new Image
                                    {
                                        ImageID = newImageID,
                                        RoomID = room.RoomID,
                                        ImageUrl = "/img/" + imageFile.FileName
                                    };

                                    db.Images.Add(newImage);
                                }
                                imageIndex++;
                            }
                        }
                        // Xóa các ảnh dư thừa
                        if (imageIndex < existingRoom.Images.Count)
                        {
                            var imagesToRemove = existingRoom.Images.Skip(imageIndex).ToList();
                            db.Images.RemoveRange(imagesToRemove);
                        }
                    }

                    // Cập nhật danh sách dịch vụ
                    int serviceIndex = 0;
                    foreach (var serviceId in selectedServices)
                    {
                        if (serviceIndex < existingRoom.RoomServices.Count)
                        {
                            var existingService = existingRoom.RoomServices.ElementAt(serviceIndex);
                            existingService.ServiceID = serviceId;
                        }
                        else
                        {
                            existingRoom.RoomServices.Add(new RoomService
                            {
                                RoomID = room.RoomID,
                                ServiceID = serviceId
                            });
                        }
                        serviceIndex++;
                    }

                    if (serviceIndex < existingRoom.RoomServices.Count)
                    {
                        var servicesToRemove = existingRoom.RoomServices.Skip(serviceIndex).ToList();
                        db.RoomServices.RemoveRange(servicesToRemove);
                    }

                    db.Update(existingRoom); // Cập nhật thông tin phòng
                    db.SaveChanges(); // Lưu thay đổi vào database
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomID))
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

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName", room.CategoryID);
            ViewBag.Services = db.Services.ToList();
            ViewBag.SelectedServices = selectedServices; // Truyền lại dịch vụ đã chọn nếu có lỗi
            return View(room);
        }

        private bool RoomExists(string id)
        {
            return (db.Rooms?.Any(e => e.RoomID == id)).GetValueOrDefault();
        }

        [Route("Delete")]
        public IActionResult Delete(string id)
        {
            // Check role
            var roleString = HttpContext.Session.GetString("Role");
            if (Enum.TryParse(roleString, out AccountType role) && role != AccountType.Admin)
            {
                // Not permission
                return RedirectToAction("AccessDenied", "Account");
            }

            if (id == null || db.Rooms == null)
            {
                return NotFound();
            }

            // Load Room cùng các bảng liên quan
            var room = db.Rooms.Include(c => c.Category)
                               .Include(i => i.Images)
                               .Include(s => s.RoomServices)
                               .Include(f => f.RentForms)
                               .FirstOrDefault(r => r.RoomID == id);

            if (room == null)
            {
                return NotFound();
            }

            if (room.RentForms.Any())
            {
                return Content("Cannot delete room because it has associated rent forms.");
            }

            return View(room); // Chuyển đến view để xác nhận xóa
        }

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (db.Rooms == null)
            {
                return Problem("Entity set 'Rooms' is null.");
            }

            // Load Room cùng các ảnh và dịch vụ liên quan
            var room = db.Rooms.Include(r => r.Images)
                               .Include(r => r.RoomServices)
                               .FirstOrDefault(r => r.RoomID == id);

            if (room != null)
            {
                // Xóa tất cả ảnh liên quan
                db.Images.RemoveRange(room.Images);

                // Xóa tất cả dịch vụ liên quan
                db.RoomServices.RemoveRange(room.RoomServices);

                // Xóa Room
                db.Rooms.Remove(room);

                // Lưu thay đổi vào database
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}

