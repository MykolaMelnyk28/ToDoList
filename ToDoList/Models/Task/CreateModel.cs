using ToDoList.Shared.Entity;
using ToDoList.Shared.Helpers;

namespace ToDoList.Models.Task
{
	public class CreateModel
	{
		public TaskEntity Task { get; set; }
		public List<PriorityEntity> Priorities { get; set; } = PriorityEntityHelper.DefaultPriorities;
	}
}
