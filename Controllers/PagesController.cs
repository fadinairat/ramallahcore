using Microsoft.AspNetCore.Mvc;
using Ramallah.Models;
using Ramallah.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace Ramallah.Controllers
{

    public class PagesController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public PagesController(DataContext context, IConfiguration config, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _config = config;
            _environment = environment;
        }

        // Get: Pages/Details/ID
        public async Task<IActionResult> Details(int id, string? title)
        {
            //if (id == null)
            //{
            //    return RedirectToAction("NotFound", "Home");
            //}

            var pageDetails = _context.Pages.Where(a => a.PageId == id && a.Deleted == false && a.Active == true && a.Publish == true).Include(a => a.Form).Include(a => a.HtmlTemplate).FirstOrDefault();


            if (pageDetails == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            pageDetails.Views = pageDetails.Views + 1;
            _context.Update(pageDetails);
            await _context.SaveChangesAsync();

            int catId = 0;

            String route = "<a href='" + Url.Action("Index", "Home") + "' class='rtText' >الرئيسية &raquo;</a>";
            PageCategory cat = _context.PagesCategories
                .Include(a => a.Category)
                .Where(a => a.PageId == id)
                .FirstOrDefault();
            if (cat != null && cat.Category.ShowInPath == true)
            {
                catId = cat.Category.Id;
                route += " <a href='" + Url.Action("Details", "Categories", new { id = cat.CategoryId, title = cat.Category.ArName }) + "' class='rtText' >" + cat.Category.ArName + " &raquo;</a>";
            }

            if (catId == 1097)
            {
                Console.WriteLine("Inside CAtid");
                //Video Gallery
                ViewBag.Related = _context.PagesCategories.Where(a => a.CategoryId == 1097 && a.Page.Active == true && a.Page.Deleted == false && a.Page.Publish == true && (a.Page.ValidDate == null || a.Page.ValidDate >= DateTime.Now.Date))
                    .Include(a => a.Page)
                    .ToList();
            }
            else Console.WriteLine("Outside Catid");


            if (pageDetails.TemplateId != null && pageDetails.TemplateId != 0 && pageDetails.HtmlTemplate.FilePath.ToString() != "")
            {
                ViewBag.Template = pageDetails.HtmlTemplate.FilePath.ToString().Trim();
            }
            else ViewBag.Template = "~/Views/Shared/Templates/_defaultPage.cshtml";

            ViewBag.Route = route;
            ViewBag.OgImage = pageDetails.Thumb;
            return View(pageDetails);
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs([FromForm] ContactUs model)
        {
            if (ModelState.IsValid)
            {
                if (!await GoogleRecaptcha.IsReCaptchaPassedAsync(Request.Form["g-recaptcha-response"],
                    _config["GoogleReCaptcha:secret"]))
                {
                    ModelState.AddModelError(string.Empty, "You failed the CAPTCHA");
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                model.SystemDate = DateTime.Now;

                _context.Add(model);
                await _context.SaveChangesAsync();
                //_uow.GetRepository<ContactUs>().Add(model);
                //_uow.SaveChanges();
                TempData["success"] = "تم ارسال رسالتك بنجاح... سوف يتم المتابعة معك بأقرب وقت.";

                return Redirect(HttpContext.Request.Headers["Referer"]);
            }

            return Redirect(HttpContext.Request.Headers["Referer"]);
        }
    }
}
