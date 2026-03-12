using shop.DataAccess;
using shop.Entities;
using shop.Utils;

namespace shop.Services
{
    public static class AuthService
    {
        public static User CurrentUser { get; private set; }

        public static bool Login(string login, string password)
        {
            var user = new UserRepository().GetByLogin(login);
            if (user != null && PasswordHasher.Verify(password, user.Password))
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static bool IsAuthenticated => CurrentUser != null;
        public static bool IsAdmin => CurrentUser?.Role == UserRole.admin;
    }
}