using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Customer")]
    public class CustomerController : Controller
    {

        private HotelDbContext db;
        public CustomerController(HotelDbContext context)
        {
            db = context;
        }

        // Hiển thị danh sách khách hàng với khả năng tìm kiếm
        [HttpGet]
        [Route("Index")]
        public IActionResult Index(string searchPhone)
        {
            var customers = db.Customers.Include(m => m.Account).AsQueryable();

            if (!string.IsNullOrEmpty(searchPhone))
            {
                customers = customers.Where(c => c.Phone.Contains(searchPhone));
            }

            var customerList = customers.ToList();

            // Kiểm tra nếu không có khách hàng nào thỏa mãn điều kiện tìm kiếm
            if (!customerList.Any() && !string.IsNullOrEmpty(searchPhone))
            {
                ViewData["NoResultsMessage"] = $"No customer found with phone number: {searchPhone}";
            }

            ViewData["searchPhone"] = searchPhone;
            return View(customerList);
        }

        //Hien thi form tao moi khach hang
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        //action xu ly POST khi submit form tao moi khach hang
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstName,LastName,Gender,Email,Phone,Address,UserName")]Customer customer)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    db.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Xử lý ngoại lệ liên quan đến cơ sở dữ liệu
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                }
            }
            // Nếu có lỗi trong ModelState, trả lại view với các thông tin đã nhập
            return View(customer);
        }

        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            if (id == null || db.Customers == null)
            {
                return NotFound();
            }
            var customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // Xử lý POST khi submit form chỉnh sửa khách hàng
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("CustomerID,FirstName,LastName,Gender,Email,Phone,Address,UserName")]Customer customer)
        {
            if(id != customer.CustomerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(customer);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(customer);
        }

        // Hàm kiểm tra sự tồn tại của Customer
        private bool CustomerExists(string id)
        {
            return (db.Customers?.Any(e => e.CustomerID == id)) ?? false;
        }

        [Route("Delete")]
        public IActionResult Delete(string id)
        {
            if(id == null || db.Customers == null)
            {
                return NotFound();
            }
            var customer = db.Customers.FirstOrDefault(e => e.CustomerID == id);
            if(customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        [HttpPost, ActionName("Delete")]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var customer = db.Customers.Find(id);
            if(customer != null)
            {
                db.Customers.Remove(customer);               
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
