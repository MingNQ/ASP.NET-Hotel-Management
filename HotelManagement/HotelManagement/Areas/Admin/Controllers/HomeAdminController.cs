using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin")]
	[Route("Admin/Homeadmin")]
	public class HomeAdminController : Controller
	{
		[Route("")]
		[Route("Index")]
		public IActionResult Index()
		{
            var userName = HttpContext.Session.GetString("Username");

            return View();
		}
	}
}
