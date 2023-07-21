using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Shared.Entity;
using ToDoList.DB;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoList.Shared.Helpers;
using ToDoList.Models.Account;

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
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (model == null || model.User == null || model.ConfirmPassword == null)
            {
                return BadRequest();
            }

            if(model.User.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Паролі не співпадають.");
            }

            if(UserExists(model.User.Login))
            {
                ModelState.AddModelError("User.Login", "Користувач з таким логіном вже існує.");
            }

            if(EmailExists(model.User.Email))
            {
                ModelState.AddModelError("User.Email", "Користувач з такою електронною адресою вже існує.");
            }

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _db.Users.AddAsync(model.User);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving user to the database.");
                return StatusCode(500);
            }

            await SignInUser(model.User);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult SignIn()
        {
            return View(new SignInModel { User = new UserEntity { Login = "", Email = "", Password = ""} });
        }

        [HttpPost]
        public IActionResult SignIn(SignInModel model)
        {
            if(model == null || model.User == null)
            {
                return BadRequest();
            }

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            UserEntity foundUser = _db.Users.FirstOrDefault(x => x.Login == model.User.Login);
            if(foundUser == null || foundUser.Password != model.User.Password)
            {
                ModelState.AddModelError(string.Empty, "Невірний логін або пароль");
                return View();
            }

            SignInUser(foundUser);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
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
        private async Task SignInUser(UserEntity user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Login", user.Login),
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
