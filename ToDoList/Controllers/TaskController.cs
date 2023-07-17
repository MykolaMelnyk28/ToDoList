using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		public readonly ApplicationContext db;
		public readonly int? CurrentUserId;

        public TaskController(ApplicationContext context)
        {
			this.db = context;
			this.CurrentUserId = int.Parse(HttpContext.User.FindFirstValue("Id"));
        }

        public IActionResult Create()
		{
			return View(new CreateModel { Task = new TaskEntity { 
				StateId = StateEntityHelper.DefaultStates[0].Id,
				UserId = CurrentUserId 
			}});
		}

		[HttpPost]
		public async Task<IActionResult> Create(TaskEntity? task)
		{
			if (task is null)
			{
				return RedirectToAction("Error", "Home");
			}
			
			if (!db.Tasks.Contains(task))
			{
				await db.Tasks.AddAsync(task);
				await db.SaveChangesAsync();
			}

			return RedirectToAction("Index", "Home");
		}

		public IActionResult Edit()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Edit(TaskEntity? task)
		{
			db.Tasks.Update(task);
			await db.SaveChangesAsync();
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int? id)
		{
			db.Tasks.Remove(new TaskEntity { Id = id.Value });
			return RedirectToAction("Index", "Home");
		}
	}
}
