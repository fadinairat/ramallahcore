using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Ramallah.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramallah
{
    public class ApiKeyMiddleware : IMiddleware
    {
        //private readonly string apiKey;

        //public ApiKeyMiddleware(string apiKey)
        //{
        //    this.apiKey = apiKey;
        //}

        //public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        //{
        //    if (!context.Request.Headers.TryGetValue("ApiKey", out var apiKeyValue) || apiKeyValue != apiKey)
        //    {
        //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //        await context.Response.WriteAsync("Unauthorized");
        //        return;
        //    }

        //    await next(context);
        //}

        private readonly RequestDelegate _next;        
        const string APIKEY = "XApiKey";
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue(APIKEY, out
                    var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided ");
                return;
            }
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEY);
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(context);
        }
    }
}
