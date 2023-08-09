using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Entity
{
    public class UserEntity : IdentityUser<int>, IUser<int>
	{

		[RegularExpression("^[A-Za-zА-Яа-яІіЇїЄєЧч]{0,24}$", ErrorMessage = "Invalid value")]
		public string? FirstName { get; set; }

		[RegularExpression("^[A-Za-zА-Яа-яІіЇїЄєЧч]{0,24}$", ErrorMessage = "Invalid value")]
		public string? LastName { get; set; }

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
                   UserName == entity.UserName &&
                   FirstName == entity.FirstName &&
                   LastName == entity.LastName &&
                   Email == entity.Email &&
                   PhoneNumber == entity.PhoneNumber &&
                   PasswordHash == entity.PasswordHash;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, UserName, FirstName, LastName, Email, PhoneNumber, PasswordHash);
        }
    }
}
