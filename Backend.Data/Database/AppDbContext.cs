using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Database
{
    public class AppDbContext : BaseDbContext
    {
        #region Constructor
        public AppDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            
        }
        #endregion
    }
}