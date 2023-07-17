using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Entity
{
	public class TaskEntity
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
		public string Content { get; set; } = string.Empty;
		public int? PriorityId { get; set; }
		public int? UserId { get; set; }
		public int? StateId { get; set; }

		public StateEntity? Sate { get; set; }
		public PriorityEntity? Priority { get; set; }
		public UserEntity? User { get; set; }
	}
}
