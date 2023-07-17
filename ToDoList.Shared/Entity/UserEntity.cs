using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Entity
{
	public class UserEntity
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(24, MinimumLength = 6)]
		public string Login { get; set; }

		[RegularExpression("^\\w{0,24}$")]
		public string? FirstName { get; set; }

		[RegularExpression("^\\w{0,24}$")]
		public string? LastName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Phone]
		public string? Phone { get; set; }

		[Required]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\w\\d\\s]).{8,}$")]
		public string Password { get; set; }

		public IEnumerable<TaskEntity> Tasks { get; set; }
	}
}
