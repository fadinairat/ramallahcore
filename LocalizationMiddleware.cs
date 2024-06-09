using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ramallah
{
    public class LocalizationMiddleware : IMiddleware
    {
        public LocalizationMiddleware()
        {
            //_context = new DataContext();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Set the culture for the current request.
            string selectedLanguage = "ar-AE"; // Replace with your language detection logic.

            // Implement language detection and culture setting logic here.
            // For example, you can inspect request headers, cookies, or user preferences.
            string culture = context.Request.Query["culture"];
            string lang = context.Request.Query["lang"];
            if (!String.IsNullOrEmpty(culture))
            {
                selectedLanguage = culture;
            }
            else if(!String.IsNullOrEmpty(lang))
            {
                selectedLanguage = lang;    
            }
            //Console.WriteLine("The Middelware Lang is: " + selectedLanguage);

            CultureInfo cultureInfo = new CultureInfo(selectedLanguage);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // Call the next middleware in the pipeline.
            await next(context);
        }
    }
}