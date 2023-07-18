using System.ComponentModel.DataAnnotations;
using ToDoList.Shared.Entity;

namespace ToDoList.Models.Account
{
	public class SignUpModel
	{
		public UserEntity User { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        public string ConfirmPassword { get; set; }
    }
}
