using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.DB;
using ToDoList.Models;
using ToDoList.Shared.Entity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ToDoList.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationContext db;

		public HomeController(ILogger<HomeController> logger, ApplicationContext context)
		{
			_logger = logger;
			db = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult Home()
		{
			return View();
		}


		public IActionResult SignUp()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SignUp(UserEntity? user)
		{
			if(!IsValid(user))
			{
				return Unauthorized();
			}

			await db.Users.AddAsync(user);
			await db.SaveChangesAsync();

			_SignIn(user);

			return RedirectToAction("Home", "Home");
		}

		public IActionResult SignIn()
		{
			return View();
		}

		[HttpPost]
		public IActionResult SignIn(UserEntity? user)
		{
			if(!IsValid(user))
			{
				return BadRequest();
			}

			UserEntity? foundUser = db.Users.FirstOrDefault(x => x.Login == user.Login && x.Password == user.Password);
			if(foundUser is null)
			{
				return Unauthorized();
			}

			_SignIn(foundUser);
			return RedirectToAction("Home", "Home");
		}

		[NonAction]
		private IActionResult _SignIn(UserEntity? user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim("Id", user.Id.ToString()),
				new Claim("Login", user.Login),
				new Claim("Password", user.Password),
				new Claim("Email", user.Email),
				new Claim("Phone", user.Phone),
				new Claim("FirstName", user.FirstName),
				new Claim("LastName", user.LastName)
			};
			ClaimsIdentity identity = new ClaimsIdentity(claims);
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);

			return base.SignIn(principal);
		}

		[NonAction]
		private bool IsValid(UserEntity user)
		{
			if(user is null)
			{
				return false;
			} 
			else if(string.IsNullOrEmpty(user.Login))
			{
				return false;
			}
			else if(string.IsNullOrEmpty(user.Password))
			{
				return false;
			}
			return true;
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}