using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class Context: DbContext
    {
        public DbSet<Core.Models.Properties> Properties { get; set; }
        public DbSet<Core.Models.Owners> Owners { get; set; }
        public DbSet<Core.Models.Amenities> Amenities { get; set; }
        public DbSet<Core.Models.Review> Reviews { get; set; }
        public DbSet<Core.Models.Images> Images { get; set; }
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }


    }
}
