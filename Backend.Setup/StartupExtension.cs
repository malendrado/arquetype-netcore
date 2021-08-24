using Backend.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Setup
{
    public static class StartupExtension
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection iServiceCollection,
            IConfiguration configuration)
        {
            #region Data
            // iServiceCollection.Configure<AutoAperturaConfig>( settings => configuration.GetSection("AutoApertura").Bind(""));
            iServiceCollection.AddDbContext<BaseDbContext>((serviceProvider, dbContextOptionsBuilder) =>
            {
                dbContextOptionsBuilder.UseSqlServer(configuration.GetConnectionString("DB_AtencionCliente"));
            });
            #endregion

            #region Services
            //iServiceCollection.AddTransient<RestClient>();
            //iServiceCollection.AddTransient(typeof(IBaseService<>), typeof(BaseService<>));
            #endregion

            #region Clients
            //iServiceCollection.AddTransient(typeof(ISolicitudProductoClient), typeof(SolicitudProductoClient));
            //iServiceCollection.AddTransient(typeof(IRecaptchaClient), typeof(RecaptchaClient));
            #endregion

            #region Repository
            //iServiceCollection.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            //iServiceCollection.AddTransient(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            //iServiceCollection.AddTransient(typeof(IOrigenUsuarioRepository), typeof(OrigenUsuarioRepository));
            //iServiceCollection.AddTransient<UnicardClientConfig>();
            #endregion

            #region Validators
            //iServiceCollection.AddTransient<IValidator<DatosRequeridosDTO>, DatosRequeridosValidators>();
            //iServiceCollection.AddTransient<IValidator<OTPDTO>, OTPDTOValidators>();
            #endregion

            return iServiceCollection;
        }
    }
}