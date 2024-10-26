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
        public IActionResult Create([Bind("RoomID, CategoryID, Status")] Room room, List<IFormFile> RoomImages)
        {
            if (ModelState.IsValid)
            {
                Room oldRoom = db.Rooms.Find(room.RoomID);
                if (oldRoom == null)
                {
                    // Kiểm tra CategoryID có hợp lệ không
                    /*var category = db.Categories.Find(room.CategoryID);
                    if (category == null)
                    {
                        ModelState.AddModelError("CategoryID", "Loại phòng không hợp lệ.");
                        ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName", room.CategoryID);
                        return View(room);
                    }*/

                    // Lưu thông tin Room trước
                    db.Rooms.Add(room);
                    db.SaveChanges();

                    // Xử lý từng ảnh trong danh sách RoomImages
                    var lastImage = db.Images.OrderByDescending(i => i.ImageID).FirstOrDefault();
                    int lastNumber = lastImage != null ? int.Parse(lastImage.ImageID.Substring(3)) : 0;

                    foreach (var imageFile in RoomImages)
                    {
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            // Cập nhật ImageID và lưu đường dẫn ảnh
                            string newImageID = "IMG" + (lastNumber + 1).ToString("D4");
                            lastNumber++;

                            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageFile.FileName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                imageFile.CopyTo(stream);
                            }

                            // Tạo và thêm ảnh mới vào database
                            var image = new Image
                            {
                                ImageID = newImageID,
                                RoomID = room.RoomID,
                                ImageUrl = "/img/" + imageFile.FileName
                            };

                            db.Images.Add(image);
                        }
                    }

                    db.SaveChanges(); // Lưu các thay đổi vào cơ sở dữ liệu

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("RoomID", "RoomId đã tồn tại.");
                }
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName");
            return View(room);
        }

        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            if(id == null || db.Rooms == null)
            {
                return NotFound();
            }
            //var room = db.Rooms.Find(id);
            var room = db.Rooms.Include(c => c.Category)
                              .Include(i => i.Images)
                              .Include(s => s.RoomServices)
                              .Include(f => f.RentForms)
                              .FirstOrDefault(r => r.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "TypeName", room.CategoryID);
            return View(room);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("RoomID, CategoryID, Status")] Room room, List<IFormFile> RoomImages)
        {
            if (id != room.RoomID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm phòng hiện tại trong database và bao gồm danh sách hình ảnh liên quan
                    var existingRoom = db.Rooms.Include(r => r.Images).FirstOrDefault(r => r.RoomID == id);

                    if (existingRoom == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin phòng
                    existingRoom.CategoryID = room.CategoryID;
                    existingRoom.Status = room.Status;

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

