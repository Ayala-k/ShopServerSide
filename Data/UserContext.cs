using Microsoft.EntityFrameworkCore;
using serverSide.Models;

namespace serverSide.Data
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options):base (options) { }

        public DbSet<User> User { get; set; }
    }
}
