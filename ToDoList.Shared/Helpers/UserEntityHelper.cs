using System.Numerics;
using ToDoList.Shared.Entity;

namespace ToDoList.Shared.Helpers
{
    public class UserEntityHelper
    {
        public static bool IsValid(UserEntity user)
        {
            if(user == null)
            {
                return false;
            }

            if(string.IsNullOrWhiteSpace(user.Login) || string.IsNullOrWhiteSpace(user.Password))
            {
                return false;
            }

            return true;
        }

        public static bool EqualsById(UserEntity u1, UserEntity u2)
        {
            return u1.Id == u2.Id;
        }

        public static bool EqualsLoginPassword(UserEntity u1, UserEntity u2)
        {
            return u1.Id == u2.Id &&
                   u1.Login == u2.Login &&
                   u1.Password == u2.Password;
        }

        public static bool EqualsContent(UserEntity u1, UserEntity u2)
        {
            return u1.Id == u2.Id &&
                   u1.Login == u2.Login &&
                   u1.FirstName == u2.FirstName &&
                   u1.LastName == u2.LastName &&
                   u1.Email == u2.Email &&
                   u1.Phone == u2.Phone &&
                   u1.Password == u2.Password;
        }
    }
}
