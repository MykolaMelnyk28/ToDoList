using ToDoList.Shared.Entity;

namespace ToDoList.Shared.Helpers
{
	public class StateEntityHelper
	{
		public static readonly List<StateEntity> DefaultStates = new List<StateEntity>
		{
			new StateEntity {Id = 1, Name = "Open"},
			new StateEntity {Id = 2, Name = "InProgress"},
			new StateEntity {Id = 3, Name = "Closed"}
		};
	}
}
