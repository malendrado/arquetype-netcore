using System.Linq;
using Backend.Data.Entities;
using Backend.Data.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Database
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {

        }
        
        //public DbSet<OrigenUsuario> OrigenUsuario { get; set; }
        //public DbSet<ComboFormulario> ComboFormulario { get; set; }
        //public DbSet<Solicitud> Solicitudes { get; set; }
        //public DbSet<Incidencia> Incidencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
        
        #region SaveChanges
        public override int SaveChanges()
        {
            var addedEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);

            foreach (var entry in addedEntries)
            {
                if (entry.Entity is IHasSequencer)
                {
                    var sequencerEntity = entry.Entity as IHasSequencer;
                    var result = new SqlParameter("@result", System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };

                    Database.ExecuteSqlRaw($"SELECT @result = (NEXT VALUE FOR {sequencerEntity.getSequenser()})", result);
                    sequencerEntity.id = (int)result.Value;
                }

                if (entry.Entity is IHasCorrelativo)
                {
                    // var TblName= AttributeReader.GetTableName<n>(this);
                    
                    var correlativoEntity = entry.Entity as IHasCorrelativo;
                    var result = new SqlParameter("@result", System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };

                    Database.ExecuteSqlRaw($"SELECT @result = (SELECT top 1 id FROM {AttributeReader.GetTableName(this, entry.Entity.GetType().Name)} ORDER BY id DESC)", result);
                    correlativoEntity.id = (int)result.Value + 1;
                    
                }
            }
            return base.SaveChanges();
        }
        #endregion
    }
}