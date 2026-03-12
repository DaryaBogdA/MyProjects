using System.Collections.Generic;

namespace shop.Entities
{
    public enum UserRole
    {
        admin,
        user
    }

    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}