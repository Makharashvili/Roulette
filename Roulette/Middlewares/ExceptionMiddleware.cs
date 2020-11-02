using Common.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Api.Helpers
{
    public class ExceptionMiddleware
    {
        ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _request;
        public ExceptionMiddleware(RequestDelegate request, ILogger<ExceptionMiddleware> logger)
        {
            _request = request;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _request(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GlobalExceptionMiddleware");
                var respModel = new BaseResponseModel { DeveloperMessage = JsonConvert.SerializeObject(ex), ErrorCode = ErrorCode.Internal, Success = false };
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(respModel));
            }
        }
    }

    public static class GlobalExceptionMiddleware
    {
        public static void UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }

}
