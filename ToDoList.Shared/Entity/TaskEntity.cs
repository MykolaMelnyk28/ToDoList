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

		public StateEntity? State { get; set; }
		public PriorityEntity? Priority { get; set; }
		public UserEntity? User { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is TaskEntity entity &&
                   Id == entity.Id &&
                   Name == entity.Name &&
                   Content == entity.Content &&
                   PriorityId == entity.PriorityId &&
                   UserId == entity.UserId &&
                   StateId == entity.StateId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Content, PriorityId, UserId, StateId);
        }
    }
}
