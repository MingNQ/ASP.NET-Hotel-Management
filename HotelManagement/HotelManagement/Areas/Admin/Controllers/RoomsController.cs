using HotelManagement.Data;
using HotelManagement.Models;
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
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public IActionResult Create([Bind("RoomID, CategoryID, Status")] Room room, IFormFile RoomImage)
        {
            if (ModelState.IsValid)
            {
                Room oldRoom = db.Rooms.Find(room.RoomID);
                if (oldRoom == null)
                {
                    // Kiểm tra CategoryID có hợp lệ không
                    var category = db.Categories.Find(room.CategoryID);
                    if (category == null)
                    {
                        ModelState.AddModelError("CategoryID", "Loại phòng không hợp lệ.");
                        ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName", room.CategoryID);
                        return View(room);
                    }

                    // Xử lý upload ảnh
                    var lastImage = db.Images.OrderByDescending(i => i.ImageID).FirstOrDefault();
                    string newImageID = lastImage == null ? "IMG0001" : "IMG" + (int.Parse(lastImage.ImageID.Substring(3)) + 1).ToString("D4");

                    if (RoomImage != null && RoomImage.Length > 0)
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", RoomImage.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            RoomImage.CopyTo(stream);
                        }

                        var image = new Image
                        {
                            ImageID = newImageID,
                            RoomID = room.RoomID,
                            ImageUrl = "/img/" + RoomImage.FileName
                        };

                        db.Images.Add(image);
                    }

                    db.Rooms.Add(room);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("RoomID", "RoomId đã tồn tại.");
                }
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName");
            return View();
        }

        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            if(id == null || db.Rooms == null)
            {
                return NotFound();
            }
            var room = db.Rooms.Find(id);
            if(room == null)
            {
                return NotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName", room.CategoryID);
            return View(room);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("RoomID, CategoryID, Status")] Room room, IFormFile RoomImage)
        {
            if (id != room.RoomID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm phòng trong database
                    var existingRoom = db.Rooms.Include(r => r.Images).FirstOrDefault(r => r.RoomID == id);

                    if (existingRoom == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin phòng
                    existingRoom.CategoryID = room.CategoryID;
                    existingRoom.Status = room.Status;

                    // Xử lý upload ảnh nếu có ảnh mới
                    if (RoomImage != null && RoomImage.Length > 0)
                    {
                        var existingImage = existingRoom.Images.FirstOrDefault();
                        // Tạo đường dẫn lưu ảnh mới
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", RoomImage.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            RoomImage.CopyTo(stream);
                        }

                        // Cập nhật hoặc tạo mới đối tượng Image
                        if (existingImage != null)
                        {
                            existingImage.ImageUrl = "/img/" + RoomImage.FileName;
                        }
                        else
                        {
                            var lastImage = db.Images.OrderByDescending(i => i.ImageID).FirstOrDefault();
                            string newImageID = "IMG0001"; // Default nếu không có ảnh nào
                            if (lastImage != null)
                            {
                                int lastNumber = int.Parse(lastImage.ImageID.Substring(3));
                                newImageID = "IMG" + (lastNumber + 1).ToString("D4");
                            }

                            var newImage = new Image
                            {
                                ImageID = newImageID,
                                RoomID = room.RoomID,
                                ImageUrl = "/img/" + RoomImage.FileName
                            };

                            db.Images.Add(newImage);
                        }
                    }

                    db.Update(existingRoom); // Cập nhật thông tin phòng
                    db.SaveChanges(); // Lưu thay đổi
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
            return View(room);
        }
        private bool RoomExists(string id)
        {
            return (db.Rooms?.Any(e => e.RoomID == id)).GetValueOrDefault();
        }
        [Route("Delete")]
        public IActionResult Delete(string id)
        {
            if(id == null || db.Rooms == null)
            {
                return NotFound();
            }

            var room = db.Rooms.Include(c => c.Category)
                               .Include(i => i.Images)
                               .Include(s => s.RoomServices)
                               .Include(f => f.RentForms)
                               .FirstOrDefault(r => r.RoomID == id);
            if(room == null)
            {
                return NotFound();
            }
            if (room.RoomServices.Count() > 0)
            {
                return Content("This room has room service, please remove the room service before deleting the room.");
            }
            if(room.RentForms.Count() > 0)
            {
                return Content("This room has room RentForm, can't delete!");
            }
            return View(room);
        }
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (db.Rooms == null)
            {
                return Problem("Entity set 'Rooms' is null");
            }
            // Tìm phòng cần xóa và bao gồm các ảnh liên quan
            var room = db.Rooms.Include(r => r.Images).FirstOrDefault(r => r.RoomID == id);

            if (room != null)
            {
                // Xóa các bản ghi ảnh liên quan khỏi cơ sở dữ liệu
                foreach (var image in room.Images)
                {
                    db.Images.Remove(image);
                }

                // Xóa phòng khỏi cơ sở dữ liệu
                db.Rooms.Remove(room);
                db.SaveChanges();
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

