using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Entity
{
	public class TaskEntity
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
		public string Content { get; set; }
		public int? PriorityId { get; set; }
		public PriorityEntity Priority { get; set; }
	}
}
