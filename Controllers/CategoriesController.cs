using Microsoft.AspNetCore.Mvc;
using Ramallah.Models;
using Ramallah.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Ramallah.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;

        public CategoriesController(DataContext context)
        {
            _context = context;
        }

        // Get: Categories/Details/ID
        public async Task<IActionResult> Details(int? id, string? title, int page = 1)
        {

            if (id == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            int skipVal = 0;
            String route = "<a href='" + Url.Action("Index", "Home") + "' >الرئيسية &raquo;</a>";
            ViewBag.Route = route;


            var catDetails = _context.Categories.Where(a => a.Id == id && a.Deleted == 0 && a.Active == true)
                .Include(a => a.HtmlTemplate)
                .FirstOrDefault();
            if (catDetails == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            ViewBag.OgImage = catDetails.Thumb;
            if (catDetails.TypeId == 1)
            {
                //If Page Category
                int PageSize = catDetails.ItemsPerPage;
                var pageList = _context.PagesCategories.Where(a => a.CategoryId == id && a.Page.Active == true && a.Page.Deleted == false && a.Page.Publish == true && (a.Page.ValidDate == null || a.Page.ValidDate >= DateTime.Now.Date))
                    .Include(a => a.Page)
                    .OrderByDescending(a => a.Page.Sticky)
                    .OrderByDescending(a => a.Page.PageDate)
                    .OrderByDescending(a => a.Page.PageId)
                    .Skip((page - 1) * PageSize)
                   .Take(PageSize);

                int dbPages = _context.PagesCategories.Where(a => a.CategoryId == id && a.Page.Active == true && a.Page.Deleted == false && a.Page.Publish == true && (a.Page.ValidDate == null || a.Page.ValidDate >= DateTime.Now.Date))
                 .Count();

                if (catDetails.TemplateId != null && catDetails.TemplateId != 0 && catDetails.HtmlTemplate.FilePath != "")
                {
                    ViewBag.Template = catDetails.HtmlTemplate.FilePath.ToString().Trim();
                }
                else ViewBag.Template = "~/Views/Shared/Templates/_defaultCat.cshtml";

                double paging = (double)dbPages / PageSize;
                double TotalPages = Math.Ceiling(paging);
                ViewBag.PagesCount = TotalPages;
                ViewBag.DBPages = dbPages;
                ViewBag.Page = page;
                ViewBag.pageList = pageList;
                ViewBag.CatDetails = catDetails;

                return View(catDetails);
            }
            else if (catDetails.TypeId == 2)
            {
                //File Category

            }
            else if (catDetails.TypeId == 3)
            {
                //Photos

            }
            else if (catDetails.TypeId == 4)
            {
                //Video Gallery
            }
            else if (catDetails.TypeId == 5)
            {
                //Audio Gallery
            }
            return View(catDetails);
        }

        public async Task<IActionResult> Gallery(int? id, int page = 1)
        {
            String route = "<a href='" + Url.Action("Index", "Home") + "' >الرئيسية &raquo;</a>";


            if (id == null)
            {//No Gallery Selected (View All Galleries)
                ViewBag.Route = route;
                int PageSize = 10;
                var galList = _context.Categories.Where(a => a.ParentId == 34 && a.Active == true && a.Deleted == 0)
                    .OrderByDescending(a => a.Priority)
                    .Skip((page - 1) * PageSize)
                   .Take(PageSize);

                int dbPages = _context.Categories.Where(a => a.ParentId == 34 && a.Active == true && a.Deleted == 0)
                 .Count();

                ViewBag.Template = "~/Views/Shared/Templates/_defaultGalList.cshtml";

                double paging = (double)dbPages / PageSize;
                double TotalPages = Math.Ceiling(paging);
                ViewBag.PagesCount = TotalPages;
                ViewBag.DBPages = dbPages;
                ViewBag.Page = page;
                ViewBag.galList = galList;

                return View("Gallery");
            }
            else
            {
                var catDetails = _context.Categories.Where(a => a.Id == id && a.Deleted == 0 && a.Active == true)
                .Include(a => a.HtmlTemplate)
                .FirstOrDefault();

                List<Files> images = _context.Files.Where(a => a.Deleted == 0 && a.CatId == id).ToList();

                ViewBag.Route = "<a href='" + Url.Action("Index", "Home") + "' >الرئيسية &raquo;</a> <a href='/Categories/Gallery' >مكتبة الصور &raquo;</a>";

                ViewBag.CatDetails = catDetails;
                ViewBag.Images = images;
                ViewBag.ImagesCount = images.Count;

                // return Json(new
                // {
                //     galleries = ViewBag.Images
                // });
                return View("GalPhotos");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}
