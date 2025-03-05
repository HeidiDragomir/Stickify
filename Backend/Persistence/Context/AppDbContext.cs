using Microsoft.EntityFrameworkCore;

namespace Backend.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }



    }
}
