using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Entity
{
    public class UserEntity
	{
		[BindNever]
		[Key]
        [HiddenInput]
        public int Id { get; set; }

		[Required(ErrorMessage = "Enter login")]
		[StringLength(24, MinimumLength = 6)]
		public string Login { get; set; }

		[RegularExpression("^[A-Za-zА-Яа-яІіЇїЄєЧч]{0,24}$", ErrorMessage = "Invalid value")]
		public string? FirstName { get; set; }

		[RegularExpression("^[A-Za-zА-Яа-яІіЇїЄєЧч]{0,24}$", ErrorMessage = "Invalid value")]
		public string? LastName { get; set; }

		[Required(ErrorMessage = "Enter login")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; }

		[RegularExpression("^(\\+\\d{1,3})?(\\d{10})$", ErrorMessage = "Invalid value")]
		public string? Phone { get; set; }

		[Required(ErrorMessage = "Enter password")]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\w\\d\\s]).{8,}$", ErrorMessage = "Invalid password")]
		public string Password { get; set; }

        [BindNever]
        public IEnumerable<TaskEntity>? Tasks { get; set; }

        public override bool Equals(object? obj)
        {
            if(this == obj)
                return true;
            else if(obj == null || obj is not UserEntity || GetHashCode() != obj.GetHashCode())
                return false;

            UserEntity entity = obj as UserEntity;

            return Id == entity.Id &&
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
