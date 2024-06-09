using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Ramallah.Models;
using Ramallah.Services;
using Ramallah.Settings;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Localization.Routing;
using Serilog;
using Serilog.Events;


var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<Ramallah.LocalizationMiddleware>();

//The AddJsonOptions to enable the DB object serialization to allow max length
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()  // Set minimum log level to Error
                           //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
//to Enable componenets and there views
builder.Services.AddControllersWithViews()
.AddRazorOptions(options =>
{
    options.ViewLocationFormats.Add("/{0}.cshtml");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Control/Users/Login";
    options.AccessDeniedPath = "/Control/Users/AccessDenied";

    options.Events = new CookieAuthenticationEvents()
    {
        //OnSigningIn = async context =>
        //{
        //    //Add Role Claim Identity before login is done
        //    var principal = context.Principal;
        //    if(principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
        //    {
        //        if(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "fadi@intertech.ps")
        //        {
        //            var claimsIdentity = principal.Identity as ClaimsIdentity;
        //            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        //        }
        //    }
        //    await Task.CompletedTask;
        //},
        //OnSignedIn = async conext =>
        //{
        //    await Task.CompletedTask;
        //},
        //OnValidatePrincipal = async context  =>
        //{
        //    await Task.CompletedTask;
        //}
    };
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins, builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


////////////////  Configure the localization for website translation //////////////
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ar-AE");

    var cultures = new CultureInfo[]
    {
        new CultureInfo("ar-AE"),
        new CultureInfo("en")
    };

    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;

    // Add supported cultures for the Area
    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider
    {
        RouteDataStringKey = "lang",
        UIRouteDataStringKey = "lang"
    });
});
//
builder.Services.AddControllersWithViews()
        .AddViewLocalization().AddDataAnnotationsLocalization();


///////////// End of Localization Settings ///////////
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/500");
}

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Home/404";
        await next();
    }
    else if (context.Response.StatusCode == 500)
    {
        context.Request.Path = "/Home/500";
        await next();
    }
    else if (context.Response.StatusCode == 503)
    {
        context.Request.Path = "/Home/500";
    }
});


app.UseRequestLocalization();
app.UseMiddleware<Ramallah.LocalizationMiddleware>();
//Enable the Localication Middleware

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(myAllowSpecificOrigins);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();   //To enable using of HttpContext.Session


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}"
    );

    //endpoints.MapControllerRoute("default", "ar/{controller=Home}/{action=Index}/{id?}/{Title?}");
    endpoints.MapControllerRoute("default", "{lang=ar}/{controller=Home}/{action=Index}/{id?}/{Title?}");
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}/{Title?}");
});
app.MapRazorPages();

app.Run();