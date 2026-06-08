using Boyr.Entities;

namespace Boyr.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByLogin(string login);
        bool LoginExists(string login);
        User Authenticate(string login, string password);
    }
}