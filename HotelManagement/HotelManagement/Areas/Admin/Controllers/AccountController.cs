using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

		[Route("")]
		[Route("Index")]
		public IActionResult Index()
		{
			var roleString = HttpContext.Session.GetString("Role");
			if (!Enum.TryParse(roleString, out AccountType role) && role == AccountType.Admin)
			{
				// Người dùng có quyền Admin
				return RedirectToAction("AccessDenied", "Account");
			}

			var accounts = db.Accounts.Include(a => a.Staff).ToList();
			return View(accounts);
		}

		[HttpPost, ActionName("ActiveAccount")]
		[Route("ActiveAccount")]
		public IActionResult ActiveAccount(int id, string action)
		{
            var accounts = db.Accounts.Include(a => a.Staff).ToList();

            if (id == null)
			{
                return View("Index", accounts);
            }
			var account = db.Accounts.Where(a => a.AccountID == id).FirstOrDefault();
			if (account != null)
			{
				try
				{
                    account.Active = action == "active" ? true : false;
                    db.Entry(account).State = EntityState.Modified;
                    db.SaveChanges();
                }
				catch
				{
					throw;
				}
			}
            return View("Index", accounts);
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
		[ValidateAntiForgeryToken]
        public IActionResult Login(Account user)  
		{
			string errorMessage = "";

			if (HttpContext.Session.GetString("Username") == null)
			{
				// Get username
				var u = db.Accounts.Where(x => x.Username.Equals(user.Username)).FirstOrDefault();

                if (u != null)
				{
					// Check Hash password
					var passwordHasher = new PasswordHasher<Account>();
					var result = passwordHasher.VerifyHashedPassword(u, u.Password, user.Password);
					
					if (result == PasswordVerificationResult.Success)
					{
                        if (u.Active == false)
                        {
                            errorMessage = "This account is not Active";
                        }
                        else
                        {
                            var staff = db.Staffs.Where(s => s.AccountID == u.AccountID).FirstOrDefault();
                            var username = staff != null ? $"{staff.FirstName} {staff.LastName}" : u.Username.ToString();

                            HttpContext.Session.SetString("Username", username);
							HttpContext.Session.SetString("Role", u.Type.ToString());

                            return RedirectToAction("Index", "HomeAdmin");
                        }
                    }
				}
				else
				{
					errorMessage = "Username or Password is incorrect!";
                }
			}

			if (!db.Accounts.Any(x => x.Username == user.Username))
			{
				errorMessage = "This account does not exist!";
			}

			ViewBag.ErrorMessage = errorMessage;
			return View();
		}

		[Route("Logout")]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			HttpContext.Session.Remove("Username");

            return RedirectToAction("Login", "Account");
		}

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
