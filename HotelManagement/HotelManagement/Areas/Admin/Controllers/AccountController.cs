using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("account")]
	public class AccountController : Controller
	{
		[Route("")]
		[Route("login")]
		public IActionResult Login()
		{
			return View();
		}

		[Route("register")]
		public IActionResult Register()
		{
			return View();
		}
	}
}
