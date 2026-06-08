using Boyr.Entities;
using Boyr.Delegates;  

namespace Boyr.Interfaces
{
    public interface IAuthService
    {
        User CurrentUser { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }

        bool Login(string login, string password);
        void Logout();

        event LoginAttemptEventHandler LoginAttempt;
    }
}