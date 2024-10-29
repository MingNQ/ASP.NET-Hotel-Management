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
        [Route("")]
        [Route("Index")]
        public IActionResult Index(string searchPhone, int page = 1)
        { 
            int pageSize = 5;
            var customers = db.Customers.Include(m => m.Account).AsQueryable();

            //tim kiem theo so diem thoai
            if (!string.IsNullOrEmpty(searchPhone))
            {
                customers = customers.Where(c => c.Phone.Contains(searchPhone));
            }


            int totalCustomers = customers.Count();

            var customerList = customers.OrderBy(c => c.CustomerID)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();


            // Kiểm tra nếu không có khách hàng nào thỏa mãn điều kiện tìm kiếm
            if (!customerList.Any() && !string.IsNullOrEmpty(searchPhone))
            {
                ViewData["NoResultsMessage"] = $"No customer found with phone number: {searchPhone}";
            }

            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCustomers / pageSize);
            ViewData["CurrentPage"] = page;
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
        public IActionResult Create([Bind("FirstName,LastName,Gender,Email,Phone,Address,Membership")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                //lay khach hang cuoi cung trong danh sach de tao ID moi
                var lastCustomer = db.Customers.OrderByDescending(c => c.CustomerID).FirstOrDefault();

                //kiem tra neu co khach hang, neeus khoong thi gan ID dau tien la CUS00001
                int nextIDNumber = 1;

                if (lastCustomer != null && lastCustomer.CustomerID.StartsWith("CUS"))
                {
                    //lay phan so tu CustomerID (bo qua 3 ky tu dau "CUS")
                    string numberPart = lastCustomer.CustomerID.Substring(3);

                    //chuyen phan so thanh kieu int va tang them 1
                    if (int.TryParse(numberPart, out int parsedNumber))
                    {
                        nextIDNumber = parsedNumber + 1;
                    }
                }

                //gan ID moi cho khach hang, dinh dang so voi 5 chu so
                customer.CustomerID = "CUS" + nextIDNumber.ToString("D5");

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
        public IActionResult Edit(string id, [Bind("CustomerID,FirstName,LastName,Gender,Email,Phone,Address,Membership")]Customer customer)
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
