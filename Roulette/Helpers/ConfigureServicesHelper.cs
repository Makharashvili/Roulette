using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Text;

namespace Roulette.Helpers
{
    public static class ConfigureServicesHelper
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, string key)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                  .AddJwtBearer(x =>
                  {
                      x.RequireHttpsMetadata = false;
                      x.SaveToken = true;
                      x.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuerSigningKey = true,
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                          ValidateIssuer = false,
                          ValidateAudience = false,
                      };
                  });

            return services;
        }

        public static IServiceCollection AddNswagDocument(this IServiceCollection services)
        {
            services.AddSwaggerDocument(config =>
            {
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                config.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Format should be 'Bearer jwt'. replace jwt with valid token",
                    In = OpenApiSecurityApiKeyLocation.Header,
                }));
            });

            return services;
        }
    }
}
