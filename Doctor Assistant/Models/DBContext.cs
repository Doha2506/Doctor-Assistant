using Microsoft.EntityFrameworkCore;

namespace Doctor_Assistant.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> doctors { get; set; }

        public DbSet<department> departments { get; set; }

        public DbSet<patient> patients { get; set; }

        public DbSet<ray> rays { get; set; }

        public DbSet<StrokeDisease> strokeDisease { get; set; }
    }
}
