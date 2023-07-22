using System.ComponentModel.DataAnnotations;
using ToDoList.Shared.Entity;

namespace ToDoList.Models.Profile
{
    public class IndexModel
    {
        public UserEntity User { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\w\\d\\s]).{8,}$", ErrorMessage = "Invalid password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}