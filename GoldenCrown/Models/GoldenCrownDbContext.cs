using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Models
{
    public class GoldenCrownDbContext : DbContext
    {
        public GoldenCrownDbContext(DbContextOptions<GoldenCrownDbContext> options) : base(options)
        {
            
        }
    }
}
