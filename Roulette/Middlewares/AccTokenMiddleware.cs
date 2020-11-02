using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ServiceModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Helpers
{
    public class AccTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtConfiguration _jwtCfg;

        public AccTokenMiddleware(RequestDelegate next, JwtConfiguration jwtCfg)
        {
            _next = next;
            _jwtCfg = jwtCfg;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await _next(context);
            }
            else
            {
                var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier,userId) };
                var tokenExpiresAt = DateTime.Now.AddMinutes(_jwtCfg.LifetimeMin);
                
                var token = TokenHelper.CreateToken(_jwtCfg.Key, tokenExpiresAt, userClaims);
                
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;
                    if (httpContext.Response.Headers.Any(x=> x.Key == "X-Response-Access-Token"))
                    {
                        httpContext.Response.Headers["X-Response-Access-Token"] = new[] { token };
                    }
                    else
                    {
                        httpContext.Response.Headers.Add("X-Response-Access-Token", new[] { token });
                    }
                
                    return Task.CompletedTask;
                }, context);

                await _next(context);
            }
        }
    }

    public static class GlobalTokenMiddleware
    {
        public static void UseGlobalTokenMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<AccTokenMiddleware>();
        }
    }
}
