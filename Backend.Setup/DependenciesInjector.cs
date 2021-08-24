using System;
using System.Linq;
using System.Reflection;
using Backend.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Setup
{
    public static class DependenciesInjector
    {
        #region Methods
        public static void AddIInjectableDependencies(this IServiceCollection services, Type ObjectType)
        {
            var types = (from t in ObjectType.Assembly.GetTypes()
                where t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract && t.GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(IInjectable))
                select (t)).OrderBy(p => p.Name).ToList();

            foreach (var type in types)
            {
                var max = 0;
                Type interfaceType = null;
                foreach (var it in type.GetInterfaces())
                {
                    var nombreInterfaceImplIService = it.GetInterfaces().Length;
                    if (it.GetInterfaces().Any(i => i == typeof(IInjectable)) && max < nombreInterfaceImplIService)
                    {
                        max = nombreInterfaceImplIService;
                        interfaceType = it;
                    }
                }
                services.AddTransient(interfaceType, type);
            }
        }
        #endregion
    }
}