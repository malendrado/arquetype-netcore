using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend.Api.Filters.OpenAPI
{
    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        #region Methods
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.Select(path => new { Key = path.Key.Replace("v{version}", swaggerDoc.Info.Version), path.Value }).ToList();

            swaggerDoc.Paths = new OpenApiPaths();
            foreach (var it in paths)
            {
                swaggerDoc.Paths.Add(it.Key, it.Value);
            }
        }
        #endregion
    }
}