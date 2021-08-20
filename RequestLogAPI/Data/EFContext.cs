using Microsoft.EntityFrameworkCore;

namespace RequestLogAPI.Data
{
    public class EFContext : DbContext
    {
        public DbSet<RequestLog> RequestLogs { get; set; }

        public EFContext(DbContextOptions<EFContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
