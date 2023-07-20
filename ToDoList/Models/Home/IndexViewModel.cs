using ToDoList.Shared;
using ToDoList.Shared.Entity;

namespace ToDoList.Models.Home
{
	public class IndexViewModel
    {   
        public IEnumerable<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
		public string SortBy { get; set; } = "Name";
		public bool IsDescending { get; set; } = false;
		public string SearchBy { get; set; } = "Name";
		public string SearchValue { get; set; } = "";
	}
}
