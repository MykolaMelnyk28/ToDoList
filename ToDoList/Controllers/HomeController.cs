using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using ToDoList.DB;
using ToDoList.Models;
using ToDoList.Models.Home;
using ToDoList.Shared.Entity;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
        }

        public async Task<IActionResult> Index()
        {
			List<TaskEntity> tasks = new List<TaskEntity>();
			if(User.Identity.IsAuthenticated)
			{
                tasks = _db.Tasks
					.Where(x => x.UserId == GetCurrentUserEntity().Id)
                    .Include(x => x.State)
					.Include(x => x.Priority)
					.ToList();
            }
            
            return View(new IndexViewModel { Tasks = tasks });
        }

        [NonAction]
        private UserEntity GetCurrentUserEntity()
        {
            ClaimsPrincipal claims = HttpContext.User;
            return _db.Users.FirstOrDefault(x => x.Id == int.Parse(claims.FindFirstValue("Id")));
        }

        [HttpPost]
		public async Task<IActionResult> Sort(IndexViewModel model)
		{
			IQueryable<TaskEntity> query = _db.Tasks.Where(x => x.UserId == GetCurrentUserEntity().Id);

			switch(model.SortBy)
			{
				case "Priority":
					query = model.IsDescending ? query.OrderByDescending(t => t.PriorityId) : query.OrderBy(t => t.PriorityId);
					break;
				case "State":
					query = model.IsDescending ? query.OrderByDescending(t => t.StateId) : query.OrderBy(t => t.StateId);
					break;
				default:
                    query = model.IsDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name);
                    break;
            }

			List<TaskEntity> tasks = await query.Include(x => x.State)
											   .Include(x => x.Priority)
											   .ToListAsync();

			return View("Index", new IndexViewModel { Tasks = tasks, SortBy = model.SortBy, IsDescending = model.IsDescending });
		}

		[HttpPost]
		public IActionResult Search(IndexViewModel model)
		{
			IQueryable<TaskEntity> query = _db.Tasks
				.Where(x => x.UserId == GetCurrentUserEntity().Id)
				.Include(x => x.State)
				.Include(x => x.Priority);

			if (!string.IsNullOrWhiteSpace(model.SearchValue))
			{
				switch(model.SearchBy)
				{
					case "Priority":
						query = query.Where(x => x.Priority.Name.StartsWith(model.SearchValue));
						break;
					case "State":
						query = query.Where(x => x.State.Name.StartsWith(model.SearchValue));
						break;
					default:
						query = query.Where(x => x.Name.StartsWith(model.SearchValue));
						break;
				}
			}
			

			List<TaskEntity> tasks = query.ToList();

			return View("Index", new IndexViewModel { Tasks = tasks, SortBy = model.SortBy, IsDescending = model.IsDescending, SearchBy = model.SearchBy, SearchValue = model.SearchValue });
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}