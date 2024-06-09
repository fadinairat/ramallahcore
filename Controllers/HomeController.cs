using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ramallah.Models;
using Ramallah.Helpers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Data;
using MimeKit;
using MailKit;
using Newtonsoft.Json;
using System.Reflection;
using MailKit.Net.Smtp;
using MailKit.Security;
using Ramallah.Services;
using System.Security.Policy;
using System.Drawing.Printing;
using System;
using static System.Collections.Specialized.BitVector32;
using AutoMapper.Execution;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using System.Text.RegularExpressions;

namespace Ramallah.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _mail;
        //private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(DataContext context, IConfiguration config, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, IEmailService mail, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _config = config;
            _environment = environment;
            _mail = mail;
            _localizer = localizer;
        }

        public IActionResult getAreas(int cityId)
        {
            var areas = _context.Villages.Where(a => a.Deleted == false && (a.CityId == cityId || a.Id == 9999)).OrderBy(a => a.ArName).ToList();

            return Json(new
            {
                result = true,
                areas = areas
            });
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        public IActionResult Index(string? culture)
        {
            try
            {
                ViewBag.PageTitle = _localizer["Ramallah"].Value;
            }
            catch (Exception ex)
            {
                Console.Write("Exception:" + ex.Message);
            }

            var slider = _context.Files.Where(a => a.CatId == 48 && a.Deleted == 0 && a.Publish == true && a.Active == true && a.FilePath != "")
            .Take(6)
            .ToList();
            ViewBag.Slider = slider;

            // return Json(new
            // {
            //     result = true,
            //     cats = slider
            // });


            var sql1 = _context.Pages.Where(aa => aa.PageId == 1438).FirstOrDefault();
            if (sql1 != null) ViewBag.SerImg = sql1.Thumb;

            var sql2 = _context.Categories.Where(aa => aa.Id == 68 || aa.Id == 1079 || aa.Id == 66 || aa.Id == 19).ToList();
            if (sql2 != null)
            {
                foreach (Category cat in sql2)
                {
                    if (cat.Id == 68) ViewBag.MediaImg = cat.Thumb;
                    else if (cat.Id == 1079) ViewBag.PrImg = cat.Thumb;
                    else if (cat.Id == 66) ViewBag.FacImg = cat.Thumb;
                    else if (cat.Id == 19) ViewBag.NewsImg = cat.Thumb;
                }
            }

            // ViewBag.Services = _context.PagesCategories
            //    .Include(a => a.Page)
            //    .Include(a => a.Category)
            //    .Where(a => a.CategoryId == 2 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
            //    .OrderByDescending(a => a.Page.PageDate)
            //    .OrderByDescending(a => a.Page.PageId)
            //    .Take(4)
            //    .ToList();

            // ViewBag.Jobs = _context.PagesCategories
            //     .Include(a => a.Page)
            //     .Include(a => a.Category)
            //     .Where(a => a.CategoryId == 2 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
            //     .OrderByDescending(a => a.Page.PageDate)
            //     .OrderByDescending(a => a.Page.PageId)
            //     .Take(8)
            //     .ToList();

            // ViewBag.Advs = _context.PagesCategories
            //     .Include(a => a.Page)
            //     .Include(a => a.Category)
            //     .Where(a => a.CategoryId == 2 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
            //     .OrderByDescending(a => a.Page.PageDate)
            //     .OrderByDescending(a => a.Page.PageId)
            //     .Take(8)
            //     .ToList();

            // ViewBag.Projects = _context.PagesCategories
            //      .Include(a => a.Page)
            //      .Include(a => a.Category)
            //      .Where(a => a.CategoryId == 2 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
            //      .OrderByDescending(a => a.Page.PageDate)
            //      .OrderByDescending(a => a.Page.PageId)
            //      .Take(8)
            //      .ToList();

            List<Category> Facilities = _context.Categories.Where(aaa => aaa.ParentId == 66 && aaa.Active == true && aaa.Deleted == 0)
            .OrderBy(aaa => aaa.Priority)
            .Take(4)
            .ToList();

            // return Json(new
            // {
            //     result = true,
            //     cats = Facilities
            // });
            ViewBag.Facilities = Facilities;


            ViewBag.Projects = _context.PagesCategories
            .Include(a => a.Page)
            .Include(a => a.Category)
            .Where(a => a.CategoryId == 1079 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
            .OrderByDescending(a => a.Page.PageDate)
            .OrderByDescending(a => a.Page.PageId)
            .Take(4)
            .ToList();


            var lastGal = _context.Categories.Where(aaa => aaa.ParentId == 34 && aaa.Active == true && aaa.Deleted == 0)
            .OrderBy(aaa => aaa.Priority)
            .FirstOrDefault();
            if (lastGal != null)
            {
                if (lastGal.Thumb != "") ViewBag.lastGal = lastGal.Thumb;
                else
                {
                    var lastImg = _context.Files.Where(aaa => aaa.CatId == lastGal.Id)
                    .OrderBy(aaa => aaa.Priority)
                    .FirstOrDefault();
                    if (lastImg != null) ViewBag.lastGal = lastImg.FilePath;
                }
            }

            ViewBag.lastVid = _context.Categories.Where(aaa => aaa.ParentId == 1097 && aaa.Active == true && aaa.Deleted == 0)
            .OrderBy(aaa => aaa.Priority)
            .FirstOrDefault();

            ViewBag.Title = "الرئيسية";
            /*ViewBag.Projects = _context.Members.Where(a => a.Deleted == false && a.Active == true).OrderByDescending(a => a.Id)
                .Take(4)
                .ToList();*/
            //ViewData["Title"] = _stringLocalizer["page.title"].Value; 
            return View();
        }

        public IActionResult Search(string keyword, int? category, DateTime? fromdate, DateTime? todate, int page = 1)
        {
            String route = "<a href='" + Url.Action("Index", "Home") + "' >الرئيسية &raquo;</a>";
            ViewBag.Route = route;
            ViewBag.category = category;
            ViewBag.keyword = keyword;
            ViewBag.fromdate = fromdate;
            ViewBag.todate = todate;
            ViewBag.page = page;
            IList<PageCategory> pagelist = null;

            ViewBag.Categories = _context.Categories.Where(a => a.Deleted == 0 && a.Active == true && a.Publish == true && a.ShowInCatList == true)
              .OrderBy(a => a.ArName)
              .ToList();

            //ViewBag.newslist = _context.PagesCategories
            //    .Include(a => a.Page)
            //    .Include(a => a.Category)
            //    .Where(a => a.CategoryId == 10 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
            //    .ToList();

            if (keyword != null || category != null || fromdate != null || todate != null)
            {
                int PageSize = 20;
                pagelist = _context.PagesCategories
                   .Where(a => a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false && a.Page.ShowInSearchPage == true &&
                   a.Page.Title.Contains(keyword) &&
                   (category == null || a.CategoryId == category) &&
                   (fromdate == null || a.Page.PageDate >= fromdate) &&
                   (todate == null || a.Page.PageDate <= todate) &&
                   (a.Page.ValidDate >= DateTime.Now.Date || a.Page.ValidDate == null)
                   )
                   .Include(a => a.Page)
                   .Include(a => a.Category)
                   .OrderByDescending(a => a.Page.PageDate)
                   .Skip((page - 1) * PageSize)
                   .Take(PageSize)

                   .ToList();

                int dbPages = _context.PagesCategories
                   .Where(a => a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false && a.Page.ShowInSearchPage == true &&
                   a.Page.Title.Contains(keyword) &&
                   (category == null || a.CategoryId == category) &&
                   (fromdate == null || a.Page.PageDate >= fromdate) &&
                   (todate == null || a.Page.PageDate <= todate) &&
                   (a.Page.ValidDate >= DateTime.Now.Date || a.Page.ValidDate == null)
                   )
                   .Count();

                float paging = (float)dbPages / PageSize;
                double TotalPages = Math.Ceiling(paging);
                ViewBag.PagesCount = TotalPages;
                ViewBag.DBPages = dbPages;

                if (pagelist.Count == 0)
                {
                    pagelist = null;
                    ViewBag.Message = "لم يتم العثور على نتائج...";
                }
            }
            else
            {
                ViewBag.Message = "كلمة البحث يجب ان تكون 3 أحرف على الاقل...";
            }

            ViewBag.pagelist = pagelist;

            return View();
        }

        public IActionResult Tag(int id, string name)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var tagDetails = _context.Tags.Where(a => a.Id == id && a.Deleted == 0).Include(a => a.HtmlTemplate).FirstOrDefault();
            if (tagDetails == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var pageList = _context.TagsRel.Where(a => a.TagId == id && a.Page.Deleted == false && a.Page.Active == true && a.Page.Publish == true)
                .Include(a => a.Page)
                .OrderByDescending(a => a.Page.Priority)
                .OrderByDescending(a => a.Page.PageDate)
                .OrderByDescending(a => a.Page.PageId)
                .ToList();
            if (tagDetails.TempId != null && tagDetails.TempId != 0 && tagDetails.HtmlTemplate.FilePath.ToString() != "")
            {
                ViewBag.Template = tagDetails.HtmlTemplate.FilePath.ToString().Trim();
            }
            else
            {
                ViewBag.Template = "~/Views/Shared/Templates/_defaultTag.cshtml";
            }
            ViewBag.pageList = pageList;

            String route = "<a href='" + Url.Action("Index", "Home") + "' >الرئيسية &raquo;</a>";
            ViewBag.route = route;

            return View(tagDetails);
        }

        [HttpGet]
        [Route("Home/Comments")]
        public IActionResult Comments()
        {
            return View();
        }
        // POST: Control/Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("CreateComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([Bind("Id,Name,Email,Location,Subject,Mobile,Body,Published,Reviewed,AddTime,Deleted")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                if (!await GoogleRecaptcha.IsReCaptchaPassedAsync(Request.Form["g-recaptcha-response"],
                    _config["GoogleReCaptcha:secret"]))
                {
                    TempData["error"] = "يجب عليك التأكيد على انك لست روبوت!";
                    return View("Comments", comments);
                }


                Boolean sending = _mail.SendEmail(new EmailDto { body = "<div style='direction:rtl;text-align:right;' >تم ارسال شكوى/ملاحظة من قبل احد زوار المنصة تحتوي البيانات التالية: <br><bR>الاسم: " + comments.Name + "<br>البريد الالكتروني: " + comments.Email + "<br><br>المكان: " + comments.Location + "<br><br>الهاتف: " + comments.Mobile + "<br><br>الموضوع: " + comments.Subject + "<br><br>محتوى الشكوى:<br> " + comments.Body + "<br></div>", subject = "شكاوى", to = "eportal@Ramallah.ps" });

                comments.AddTime = DateTime.Now;
                _context.Add(comments);
                await _context.SaveChangesAsync();
                TempData["success"] = "تم اضافة تعليقك بنجاح! سوف يتم التواصل معك في أقرب وقت ممكن.";
                return RedirectToAction("Comments");
            }
            else
            {
                TempData["error"] = "خطأ في البيانات!";
                return View("Comments", comments);
            }
        }


        // [HttpGet]
        // [Route("Language")]
        public IActionResult ChangeLanguage(string culture)
        {
            //Response.Cookies.Append(
            //    CookieRequestCultureProvider.DefaultCookieName,
            //    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            //    new CookieOptions
            //    {
            //        Expires = DateTimeOffset.UtcNow.AddDays(7)
            //    }
            //);

            Console.WriteLine("Language");
            return RedirectToAction("Index");
        }

        public IActionResult Forget()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public DataSet ToDataSet<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dataTable);
            return ds;
        }


        public IActionResult Signup(int? type)
        {
            if (type == null)
            {
                return View();
            }
            else if (type == 0)//Job Seeker
            {
                ViewData["CityId"] = _context.City.OrderBy(a => a.Id).ToList();

                return View("SignupSeekers");
            }
            else if (type == 1)//Employer Signup
            {
                ViewData["CityId"] = _context.City.OrderBy(a => a.Id).ToList();

                return View("SignupEmployers");
            }

            return View();
        }



        public IActionResult Guide()
        {
            return View();
        }

        public IActionResult Video()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            ViewBag.CityId = _context.City.ToList();
            return View();
        }



        [Route("Home/404")]
        public IActionResult NotFound()
        {
            return View();
        }

        [Route("Home/500")]
        public IActionResult ErrorPage()
        {
            return View();
        }

    }


}
