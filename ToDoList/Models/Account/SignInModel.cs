using Microsoft.AspNetCore.Mvc;
using ToDoList.Shared.Entity;

namespace ToDoList.Models.Account
{
	public class SignInModel
	{
        public UserEntity User { get; set; }
	}
}
