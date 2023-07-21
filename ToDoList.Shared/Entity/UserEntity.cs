using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Entity
{
	public class UserEntity
	{
		[BindNever]
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Enter login")]
		[StringLength(24, MinimumLength = 6)]
		public string Login { get; set; }

		[RegularExpression("^\\w{0,24}$")]
		public string? FirstName { get; set; }

		[RegularExpression("^\\w{0,24}$")]
		public string? LastName { get; set; }

		[Required(ErrorMessage = "Enter login")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; }

		[Phone(ErrorMessage = "Invalid phone address")]
		public string? Phone { get; set; }

		[Required(ErrorMessage = "Enter password")]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\w\\d\\s]).{8,}$", ErrorMessage = "Invalid password")]
		public string Password { get; set; }

        [BindNever]
        public IEnumerable<TaskEntity>? Tasks { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is UserEntity entity &&
                   Id == entity.Id &&
                   Login == entity.Login &&
                   FirstName == entity.FirstName &&
                   LastName == entity.LastName &&
                   Email == entity.Email &&
                   Phone == entity.Phone &&
                   Password == entity.Password;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Login, FirstName, LastName, Email, Phone, Password);
        }
    }
}
