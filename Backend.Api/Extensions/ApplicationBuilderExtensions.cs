using Backend.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Backend.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        #region Methods
        public static IApplicationBuilder UseAppSwagger(this IApplicationBuilder _iApplicationBuilder)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            _iApplicationBuilder.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            _iApplicationBuilder.UseSwaggerUI(c =>
            {
                c.EnableFilter();
                c.ShowExtensions();
                c.EnableValidator();
                c.EnableDeepLinking();
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.RoutePrefix = string.Empty;
                c.DefaultModelExpandDepth(2);
                c.DefaultModelsExpandDepth(-1);
                c.DocExpansion(DocExpansion.None);
                c.DefaultModelRendering(ModelRendering.Model);
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Version 1");
                //c.SwaggerEndpoint($"/swagger/v2/swagger.json", "Version 2");
                //c.SwaggerEndpoint($"/swagger/v3/swagger.json", "Version 3 - beta");
                //c.InjectStylesheet("/swagger-custom/theme-outline.css");  //Added Code
                //c.InjectJavascript("/swagger-custom/swagger-custom-script.js", "text/javascript");
            });

            return _iApplicationBuilder;
        }
        
        public static IApplicationBuilder UseAppExceptionsMiddleware(this IApplicationBuilder _iApplicationBuilder)
        {
            _iApplicationBuilder.UseMiddleware<LoggingMiddleware>();
            _iApplicationBuilder.UseMiddleware<AppExceptionsMiddleware>();
            return _iApplicationBuilder;
        }

        #endregion
    }
}