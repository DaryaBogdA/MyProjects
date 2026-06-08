using Boyr.DataAccess;
using Boyr.Delegates; 
using Boyr.Entities;
using Boyr.Interfaces;
using Boyr.Utils;

namespace Boyr.Services
{
    public class AuthService : IAuthService
    {
        private static AuthService _instance;
        private User _currentUser;

        private AuthService() { }

        public static AuthService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AuthService();
                return _instance;
            }
        }

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public bool IsAdmin => _currentUser?.Role == UserRole.admin;

        public event LoginAttemptEventHandler LoginAttempt;

        public bool Login(string login, string password)
        {
            var user = new UserRepository().GetByLogin(login);
            bool success = user != null && PasswordHasher.Verify(password, user.Password);

            if (success)
            {
                _currentUser = user;
            }

            OnLoginAttempt(login, success);
            return success;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        private void OnLoginAttempt(string login, bool success)
        {
            LoginAttempt?.Invoke(login, success);
        }
    }
}