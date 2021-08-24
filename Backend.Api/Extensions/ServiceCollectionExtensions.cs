using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Backend.Api.Filters;
using Backend.Api.Filters.OpenAPI;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using NLog.Web;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iServiceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppMVC(this IServiceCollection _iServiceCollection)
        {
            _iServiceCollection.AddControllers()
                  .AddNewtonsoftJson(options =>
                  {
                      options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                  })
                  .AddMvcOptions(options =>
                  {
                      options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                  });
            return _iServiceCollection;
        }

        public static IServiceCollection AddFluent(this IServiceCollection _iServiceCollection)
        {
            _iServiceCollection
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(mvcConfiguration => mvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);            
            return _iServiceCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iServiceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppLogging(this IServiceCollection _iServiceCollection)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true).Build();

            var logProvider = config.GetSection("LogProvider").Value;
            _iServiceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog(configFileName: logProvider);
            });

            NLog.Extensions.Logging.ConfigSettingLayoutRenderer.DefaultConfiguration = config;

            return _iServiceCollection;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iServiceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppSwaggerGen(this IServiceCollection _iServiceCollection)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            _iServiceCollection.AddSwaggerGen(c =>
            {
                c.DescribeAllEnumsAsStrings();
                
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"ASP.NET Core 5 API ambiente: {env}",
                    Description = "RESTFul ASP.NET Core 5 API",
                    Contact = new OpenApiContact
                    {
                        Name = "Luis Cortés",
                        Email = "luis@cortesdev.cl",
                        Url = new Uri("https://www.cortesdev.cl/")
                    }
                });

                c.OperationFilter<RemoveVersionFromParameter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                c.DocInclusionPredicate((version, desc) =>
                {
                    if (!desc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType.GetCustomAttributes().OfType<ApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions);

                    var maps = methodInfo.GetCustomAttributes().OfType<MapToApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions)
                    .ToArray();

                    var ver = versions.Any(v => $"v{v.ToString()}" == version)
                                  && (!maps.Any() || maps.Any(v => $"v{v.ToString()}" == version));
                    return ver;
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });

                var security = new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}

                    }
                };
                c.AddSecurityRequirement(security);
                

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            _iServiceCollection.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://localhost:44371",
                    ValidAudience = "https://localhost:44371",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Thisismysecretkey")) //Configuration["JwtToken:SecretKey"]
                };
                
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Ensure we always have an error and error description.
                        if (string.IsNullOrEmpty(context.Error))
                            context.Error = "invalid_token";
                        if (string.IsNullOrEmpty(context.ErrorDescription))
                            context.ErrorDescription = "This request requires a valid JWT access token to be provided";

                        // Add some extra context for expired tokens.
                        if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                            context.Response.Headers.Add("x-token-expired", authenticationException.Expires.ToString("o"));
                            context.ErrorDescription = $"The token expired on {authenticationException.Expires.ToString("o")}";
                        }

                        return context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            error = context.Error,
                            error_description = context.ErrorDescription
                        }));
                    }
                };
            });;
            
            

            return _iServiceCollection;
        }        
        #endregion
    }
}