using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boyr.Entities
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
