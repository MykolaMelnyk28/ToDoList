using ToDoList.Shared.Entity;

namespace ToDoList.Shared.Helpers
{
	public class PriorityEntityHelper
	{
		public static readonly List<PriorityEntity> DefaultPriorities = new List<PriorityEntity>
		{
			new PriorityEntity { Id = 1, Name = "Easy"},
			new PriorityEntity { Id = 2, Name = "Medium" },
			new PriorityEntity { Id = 3, Name = "Hard" }
		};
	}
}
