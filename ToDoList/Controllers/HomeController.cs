using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoList.Models.Home;
using ToDoList.Servicies;
using ToDoList.Shared.Entity;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TaskManager _taskManager;
        private readonly UserManager<UserEntity> _userManager;

        public HomeController(ILogger<HomeController> logger, TaskManager taskManager, UserManager<UserEntity> userManager)
        {
            _logger = logger;
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
        }

        public async Task<IActionResult> Index()
        {            
            return View(new IndexViewModel(await GetTasksAll()));
        }

		[HttpPost]
		public async Task<IActionResult> Sort(IndexViewModel model)
		{
            return View("Index", new IndexViewModel(await GetTasksAll(), sortBy: model.SortBy, isDescending: model.IsDescending));
		}

		[HttpPost]
		public async Task<IActionResult> Search(IndexViewModel model)
		{
            return View("Index", new IndexViewModel(await GetTasksAll(), searchBy: model.SearchBy, searchValue: model.SearchValue));
		}

        [NonAction]
        private async Task<UserEntity> GetCurrentUserEntity()
        {
            UserEntity? user = await _userManager.GetUserAsync(HttpContext.User);
            return user;
        }

        [NonAction]
        private async Task<IEnumerable<TaskEntity>> GetTasksAll()
        {
            List<TaskEntity> tasks = new List<TaskEntity>();
            if(User.Identity.IsAuthenticated)
            {
                UserEntity user = await GetCurrentUserEntity();
                tasks.AddRange(_taskManager.GetTasks(x => x.UserId == user.Id));
            }
            return tasks;
        }
    }
}