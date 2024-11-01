using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
