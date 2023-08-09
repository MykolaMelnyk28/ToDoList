using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<UserEntity> _userManager;

        public ProfileController(ILogger<ProfileController> logger, UserManager<UserEntity> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
        }

        public async Task<IActionResult> Index()
        {
            UserEntity user = await GetCurrentUserEntity();
            var model = new IndexModel { User = user };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(IndexModel model)
        {
            UserEntity foundUser = await GetCurrentUserEntity();

            ModelState.Remove("User");
            if(model.Password != foundUser.PasswordHash)
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

            foundUser.PasswordHash = _userManager.PasswordHasher.HashPassword(foundUser, model.Password);

            await _userManager.UpdateAsync(foundUser);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditDetails(IndexModel model)
        {
            if(model == null || model.User == null)
            {
                BadRequest();
            }

            UserEntity foundUser = await GetCurrentUserEntity();

            if(model.User.UserName != foundUser.UserName && UserExists(model.User.UserName))
            {
                ModelState.AddModelError("User.UserName", "User with this login already exists.");
            }

            if(model.User.Email != foundUser.Email && EmailExists(model.User.Email))
            {
                ModelState.AddModelError("User.Email", "User with this email already exists.");
            }

            if(!string.IsNullOrWhiteSpace(model.User.PhoneNumber) && model.User.PhoneNumber != foundUser.PhoneNumber && PhoneExists(model.User.PhoneNumber))
            {
                ModelState.AddModelError("User.PhoneNumber", "User with this phone number already exists.");
            }

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if(!ModelState.IsValid)
            {
                return View("Index", model);
            }

            UpdateUser(foundUser, model.User);
            await _userManager.UpdateAsync(foundUser);

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
                _old.UserName = _new.UserName;
                _old.Email = _new.Email;
                _old.PhoneNumber = _new.PhoneNumber ?? "";
            }
        }

        [NonAction]
        private async Task<UserEntity> GetCurrentUserEntity()
        {
            UserEntity? user = await _userManager.GetUserAsync(HttpContext.User);
            return user;
        }

        [NonAction]
        private bool PhoneExists(string phone)
        {
            return _userManager.Users.Any(x => x.PhoneNumber == phone);
        }

        [NonAction]
        private bool UserExists(string login)
        {
            return _userManager.Users.Any(x => x.UserName == login);
        }


        [NonAction]
        private bool EmailExists(string email)
        {
            return _userManager.Users.Any(x => x.Email == email);
        }

        [NonAction]
        private async Task SetUser(UserEntity user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                new Claim("FirstName", user.FirstName ?? ""),
                new Claim("LastName", user.LastName ?? "")
            };


            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
		}
    }
}
