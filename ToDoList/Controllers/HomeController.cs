using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.DB;
using ToDoList.Models;

namespace ToDoList.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationContext db;

		public HomeController(ILogger<HomeController> logger, ApplicationContext context)
		{
			_logger = logger;
			db = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}