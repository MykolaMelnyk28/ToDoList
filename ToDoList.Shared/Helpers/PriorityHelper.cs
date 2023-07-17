using ToDoList.Shared.Entity;

namespace ToDoList.Shared.Helpers
{
	public class PriorityEntityHelper
	{
		public static readonly List<PriorityEntity> DefaultPriorities = new List<PriorityEntity>
		{
			new PriorityEntity { Name = "Easy"},
			new PriorityEntity { Name = "Medium" },
			new PriorityEntity { Name = "Hard" }
		};
	}
}
