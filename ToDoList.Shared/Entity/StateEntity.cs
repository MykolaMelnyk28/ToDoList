using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Entity
{
	public class StateEntity
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }

		public IEnumerable<TaskEntity> Tasks { get; set; }
	}
}
