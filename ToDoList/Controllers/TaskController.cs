using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoList.DB;
using ToDoList.Models.Task;
using ToDoList.Shared.Entity;
using ToDoList.Shared.Helpers;

namespace ToDoList.Controllers
{
    [Authorize]
	public class TaskController : Controller
	{
        private readonly ILogger<TaskController> _logger;
        private readonly ApplicationContext _db;

        public TaskController(ILogger<TaskController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
        }

        public IActionResult Create()
		{
            int currentUserId = int.Parse(HttpContext.User.FindFirstValue("Id"));
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

            try
            {
                if(!_db.Tasks.Contains(task))
                {
                    await _db.Tasks.AddAsync(task);
                    await _db.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a task.");
                return StatusCode(500);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Edit(int? id)
        {
            TaskEntity? task = _db.Tasks.FirstOrDefault(x => x.Id == id);

            if(task == null)
            {
                return BadRequest();
            }

            return View(new EditModel { Task = task });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskEntity task)
        {
            try
            {
                var existingTask = await _db.Tasks.FindAsync(task.Id);

                if(existingTask == null)
                {
                    return NotFound();
                }

                _db.Entry(existingTask).CurrentValues.SetValues(task);
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error occurred while editing a task.");
                return RedirectToAction("ConcurrencyError");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing a task.");
                return StatusCode(500);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                _db.Tasks.Remove(new TaskEntity { Id = id.Value });
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a task.");
                return StatusCode(500);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
