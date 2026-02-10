using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bogdanovich.Domain
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
    }
}
