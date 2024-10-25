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
        public IActionResult Index()
        {
            var rooms = db.Rooms.Include(r => r.Category)
                                .Include(i => i.Images)
                                .ToList();
            return View(rooms);
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


    }
}

