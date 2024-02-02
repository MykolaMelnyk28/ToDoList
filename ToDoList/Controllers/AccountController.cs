using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using ToDoList.Models.Account;
using ToDoList.Shared.Entity;

namespace ToDoList.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<UserEntity> _userManager;

        public AccountController(ILogger<AccountController> logger, UserManager<UserEntity> userManager)
        {
            _logger = logger;
            _userManager = userManager;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (model == null || model.User == null || model.ConfirmPassword == null)
            {
                return BadRequest();
            }

            ModelState.Remove("User.Id");

            if(UserExists(model.User.UserName))
            {
                ModelState.AddModelError("User.UserName", "The user with this login already exists.");
            }

            if(EmailExists(model.User.Email))
            {
                ModelState.AddModelError("User.Email", "The user with this email already exists.");
            }

            bool isValidPassword = await ValidatePassword(model.User, model.User.PasswordHash);
            if(!isValidPassword)
            {
                ModelState.AddModelError("User.PasswordHash", "Invalid password format.");
            }

            if(model.User.PasswordHash != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
            }

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            model.User.PasswordHash = _userManager.PasswordHasher.HashPassword(model.User, model.User.PasswordHash);

            var result = await _userManager.CreateAsync(model.User);

            if(result.Succeeded)
            {
                await SignInUser(model.User);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult SignIn()
        {
            return View(new SignInModel { User = new UserEntity { UserName = "", Email = "", PasswordHash = ""} });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if(model == null || model.User == null)
            {
                return BadRequest();
            }

            ModelState.Remove("User.Id");
            ModelState.Remove("User.Email");

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            UserEntity? foundUser = await _userManager.FindByNameAsync(model.User.UserName);

            if(foundUser == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View();
            }

            var result = _userManager.PasswordHasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash, model.User.PasswordHash);
            if(result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Incorrect password");
                return View();
            }

            model.User.PasswordHash = _userManager.PasswordHasher.HashPassword(model.User, model.User.PasswordHash);

            await SignInUser(foundUser);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            UserEntity? foundUser = await _userManager.FindByIdAsync(id?.ToString());
            if (foundUser == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(foundUser);

            return await Logout();
        }

        [NonAction]
        private async Task<bool> ValidatePassword(UserEntity user, string password)
        {
            var result = await _userManager.PasswordValidators[0].ValidateAsync(_userManager, user, password);
            return result.Succeeded;
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
        private async Task SignInUser(UserEntity user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName??""),
                new Claim(ClaimTypes.Email, user.Email?? ""),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber??""),
                new Claim("FirstName", user.FirstName??""),
                new Claim("LastName", user.LastName??"")
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
