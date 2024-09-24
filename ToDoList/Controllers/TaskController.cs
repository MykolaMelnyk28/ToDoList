using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using ToDoList.DB;
using ToDoList.Models.Task;
using ToDoList.Servicies;
using ToDoList.Shared.Entity;
using ToDoList.Shared.Helpers;

namespace ToDoList.Controllers
{
    [Authorize]
	public class TaskController : Controller
	{
        private readonly ILogger<TaskController> _logger;
        private readonly TaskManager _taskManager;

        public TaskController(ILogger<TaskController> logger, TaskManager taskManager)
        {
            _logger = logger;
            _taskManager = taskManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
        }

        public IActionResult Create()
		{
            int currentUserId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(new CreateModel { Task = new TaskEntity { 
				StateId = StateEntityHelper.DefaultStates[0].Id,
				UserId = currentUserId
            }});
		}

        [HttpPost]
        public async Task<IActionResult> Create(TaskEntity? task)
        {
            if(task == null)
            {
                return BadRequest();
            }
            else if (string.IsNullOrWhiteSpace(task.Content))
            {
                task.Content = string.Empty;
            }

            if (!ModelState.IsValid)
            {
                return View("Create");
            }
            
            bool result = await _taskManager.CreateTaskAsync(task);
            return (result) ? RedirectToAction("Index", "Home") : View("Create");
        }

        public IActionResult Edit(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }

            TaskEntity? task = _taskManager.FirstOrDefalut(x => x.Id == id.Value);

            if(task == null)
            {
                return BadRequest();
            }

            return View(new EditModel { Task = task });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskEntity task)
        {
            if(task == null)
            {
                return BadRequest();
            } 
            else if (string.IsNullOrWhiteSpace(task.Content))
            {
                task.Content = string.Empty;
            }

            if (!ModelState.IsValid)
            {
                return View("Create");
            }

            bool result = await _taskManager.UpdateTaskAsync(task);
            return (result) ? RedirectToAction("Index", "Home") : View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            await _taskManager.DeleteTaskByIdAsync(id.Value);
            return RedirectToAction("Index", "Home");
        }
    }
}
