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

        public DbSet<Department> departments { get; set; }

        public DbSet<Patient> patients { get; set; }

        public DbSet<Ray> rays { get; set; }

        public DbSet<StrokeDisease> strokeDisease { get; set; }
        public DbSet<Disease> disease { get; set; }

    }
}
