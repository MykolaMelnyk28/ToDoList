using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Models.Profile;
using ToDoList.Shared.Entity;

namespace ToDoList.Controllers
{
	public class ProfileController : Controller
	{
		[Authorize]
		public IActionResult Index()
		{
			ClaimsPrincipal claims = HttpContext.User;
			return View(new IndexModel
			{
				User = new UserEntity
				{
					Id = int.Parse(claims.FindFirstValue("Id")),
					Login = claims.FindFirstValue("Login"),
					Password = claims.FindFirstValue("Password"),
					Email = claims.FindFirstValue("Email"),
					Phone = claims.FindFirstValue("Phone"),
					FirstName = claims.FindFirstValue("FirstName"),
					LastName = claims.FindFirstValue("LastName")
				}
			});
		}
	}
}
