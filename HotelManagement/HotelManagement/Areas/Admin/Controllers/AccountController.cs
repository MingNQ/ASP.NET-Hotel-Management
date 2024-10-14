using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Account")]
	public class AccountController : Controller
	{
		private HotelDbContext db;

		public AccountController(HotelDbContext context) 
		{
			db = context;
		}

		[HttpGet]
		[Route("")]
		[Route("Login")]
		public IActionResult Login()
		{
			if (HttpContext.Session.GetString("Username") == null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Index", "HomeAdmin");
			}
		}

		[HttpPost]
        [Route("")]
        [Route("Login")]
        public IActionResult Login(Account user)  
		{
			if (HttpContext.Session.GetString("Username") == null)
			{
				var u = db.Accounts.Where(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password)).FirstOrDefault();

				if (u != null)
				{
					HttpContext.Session.SetString("Username", u.Username.ToString());
					return RedirectToAction("Index", "HomeAdmin");
				}
			}
			return View();
		}

		[Route("Logout")]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			HttpContext.Session.Remove("Username");
			return RedirectToAction("Login", "Account");
		}
	}
}
