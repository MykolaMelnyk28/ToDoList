using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Shared.Entity;
using ToDoList.DB;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoList.Shared.Helpers;

namespace ToDoList.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationContext _db;

        public AccountController(ILogger<AccountController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserEntity user, string confirmPassword)
        {
            if(!UserEntityHelper.IsValid(user))
            {
                return Unauthorized();
            }

            if(user.Password != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Паролі не співпадають");
                return View();
            }

            try
            {
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving user to the database.");
                return StatusCode(500);
            }

            await _SignIn(user);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(UserEntity user)
        {
            if(!UserEntityHelper.IsValid(user))
            {
                return BadRequest();
            }

            UserEntity foundUser = _db.Users.FirstOrDefault(x => x.Login == user.Login);
            if(foundUser == null || foundUser.Password != user.Password)
            {
                ModelState.AddModelError(string.Empty, "Невірний логін або пароль");
                return View();
            }

            _SignIn(foundUser);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task _SignIn(UserEntity user)
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

            ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
