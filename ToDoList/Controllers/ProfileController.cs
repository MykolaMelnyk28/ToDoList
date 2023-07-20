using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using ToDoList.DB;
using ToDoList.Models.Profile;
using ToDoList.Shared.Entity;

namespace ToDoList.Controllers
{
	[Authorize]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationContext _db;

        public ProfileController(ILogger<ProfileController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
        }

        public IActionResult Index()
        {
            ClaimsPrincipal claims = HttpContext.User;
            UserEntity user = new UserEntity
            {
                Id = int.Parse(claims.FindFirstValue("Id")),
                Login = claims.FindFirstValue("Login"),
                Password = claims.FindFirstValue("Password"),
                Email = claims.FindFirstValue("Email"),
                Phone = claims.FindFirstValue("Phone"),
                FirstName = claims.FindFirstValue("FirstName"),
                LastName = claims.FindFirstValue("LastName")
            };
            return View(new IndexModel { User = user });
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserEntity user)
        {
            if(!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user in the database.");
                return StatusCode(500);
            }

            return RedirectToAction("Index");
        }
    }
}
