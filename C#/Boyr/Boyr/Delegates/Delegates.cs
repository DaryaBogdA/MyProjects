using Boyr.Entities;

namespace Boyr.Delegates
{
    public delegate void ProductAddedEventHandler(Product product);
    
    public delegate void ProductDeletedEventHandler(int productId);

    public delegate void ProductUpdatedEventHandler(Product product);

    public delegate void CartChangedEventHandler(int itemsCount, decimal totalAmount);

    public delegate void LoginAttemptEventHandler(string login, bool success);

    public delegate void OrderCompletedEventHandler(int saleId, decimal totalAmount);
   
    public delegate void UserRoleChangedEventHandler(int userId, string oldRole, string newRole);
}