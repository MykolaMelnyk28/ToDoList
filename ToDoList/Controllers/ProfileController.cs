using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            UserEntity user = GetCurrentUserEntity();
            var model = new IndexModel { User = user };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(IndexModel model)
        {
            UserEntity foundUser = GetCurrentUserEntity();

            ModelState.Remove("User");
            if(model.Password != foundUser.Password)
            {
                if(!IsPasswordValid(model.Password))
                {
                    ModelState.AddModelError("User.Password", "Invalid password format");
                }
            }

            if(!ModelState.IsValid)
            {
                return View("Index", model);
            }

            foundUser.Password = model.Password;
            _db.Users.Update(foundUser);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditDetails(IndexModel model)
        {
            if(model == null || model.User == null)
            {
                BadRequest();
            }

            UserEntity foundUser = GetCurrentUserEntity();

            if(model.User.Login != foundUser.Login && UserExists(model.User.Login))
            {
                ModelState.AddModelError("User.Login", "User with this login already exists.");
            }

            if(model.User.Email != foundUser.Email && EmailExists(model.User.Email))
            {
                ModelState.AddModelError("User.Email", "User with this email already exists.");
            }

            if(!string.IsNullOrWhiteSpace(model.User.Phone) && model.User.Phone != foundUser.Phone && PhoneExists(model.User.Phone))
            {
                ModelState.AddModelError("User.Phone", "User with this phone number already exists.");
            }

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if(!ModelState.IsValid)
            {
                return View("Index", model);
            }

            UpdateUser(foundUser, model.User);
            _db.Users.Update(foundUser);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [NonAction]
        private bool IsPasswordValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
        }

        [NonAction]
        private void UpdateUser(UserEntity _old, UserEntity _new)
        {
            if(!_old.Equals(_new))
            {
                _old.FirstName = _new.FirstName ?? "";
                _old.LastName = _new.LastName ?? "";
                _old.Login = _new.Login;
                _old.Email = _new.Email;
                _old.Phone = _new.Phone ?? "";
            }
        }

        [NonAction]
        private UserEntity GetCurrentUserEntity()
        {
            ClaimsPrincipal claims = HttpContext.User;
            UserEntity user = _db.Users.FirstOrDefault(x => x.Id == int.Parse(claims.FindFirstValue("Id")));
            return user;
        }

        [NonAction]
        private bool PhoneExists(string phone)
        {
            return _db.Users.Count(x => x.Phone == phone) > 0;
        }

        [NonAction]
        private bool UserExists(string login)
        {
            return _db.Users.Count(x => x.Login == login) > 0;
        }

        [NonAction]
        private bool EmailExists(string email)
        {
            return _db.Users.Count(x => x.Email == email) > 0;
        }

        [NonAction]
        private async Task SetUser(UserEntity user)
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

			ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
		}
    }
}
