using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Ramallah.Models;
using Ramallah.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;



namespace Ramallah.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [Ramallah.AuthorizedAction]
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public CategoriesController(DataContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment IHostingEnvironment, IMemoryCache cache, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _environment = IHostingEnvironment;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _localizer = localizer;
        }

        // GET: Control/Categories
        public async Task<IActionResult> Index(string? keyword, int? stype, int? slang, int PageNumber = 1)
        {
            int PageSize = 20;

            var dataContext = _context.Categories.Where(a => a.Deleted == 0
            && (keyword == null || a.Name.Contains(keyword) || a.ArName.Contains(keyword))
            && (stype == null || a.TypeId == stype)
            && (slang == null || a.LangId == slang)
            )
            .Include(c => c.HtmlTemplate)
            .Include(c => c.Language)
            .Include(a => a.Language)
            .Include(a => a.CategoryTypes)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .OrderByDescending(a => a.Id);

            int dbPages = _context.Categories.Where(a => a.Deleted == 0
            && (keyword == null || a.Name.Contains(keyword) || a.ArName.Contains(keyword))
            && (stype == null || a.TypeId == stype)
            && (slang == null || a.LangId == slang)
            )
            .Include(c => c.HtmlTemplate)
            .Include(c => c.Language)
            .Include(a => a.Language)
            .Include(a => a.CategoryTypes)
            .Count();

            float paging = (float)dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            ViewBag.Types = _context.Category_Types.ToList();
            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            ViewBag.Type = stype;
            ViewBag.Language = slang;
            ViewBag.Keyword = keyword;
            ViewBag.PagesCount = TotalPages;

           
            return View(await dataContext.ToListAsync());
        }

        // GET: Control/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var category = await _context.Categories
                .Include(c => c.HtmlTemplate)
                .Include(c => c.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            
            return View(category);
        }

        // GET: Control/Categories/Create
        public IActionResult Create()
        {
            ViewData["TemplateId"] = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 2).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture == "en" ? "Name" : "ArName"));
            ViewData["Types"] = new SelectList(_context.Category_Types, "Id", (currentCulture == "en" ? "Title" : "ArTitle"));
            ViewData["Parents"] = _context.Categories.Where(a => a.Deleted == 0 && a.ParentId == 0).ToList();

            return View();
        }

        // POST: Control/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ArName,Slug,Thumb,Priority,ParentId,TemplateId,ItemsPerPage,TypeId,Description,ArDescription,LangId,ShowAsMain,ShowInSiteMap,ShowDescription,ShowTitle,ShowThumb,ShowInPath,ShowInSearch,ShowDate,ShowInCatList")] Category category)
        {
            //ModelState.Remove("")
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("id") != "")
                {
                    category.UserId = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                }
                else category.UserId = null;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadImage(HttpContext, "files/image/CatImages/", _environment.WebRootPath);
                    category.Thumb = ImageUrl.Item1;
                }

                category.Name = Functions.RemoveHtml(category.Name);
                category.ArName = Functions.RemoveHtml(category.ArName);
                category.Slug = Functions.RemoveHtml(category.Slug);

                category.Description = Functions.FilterHtml(category.Description);
                category.ArDescription = Functions.FilterHtml(category.ArDescription);

                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["success"] = _localizer["Item Added"].Value;

                return RedirectToAction(nameof(Index));
            }
            ViewData["TemplateId"] = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 2).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture == "en" ? "Name" : "ArName"), category.LangId);
            ViewData["Types"] = new SelectList(_context.Category_Types, "Id", (currentCulture == "en" ? "Name" : "ArName"));
            ViewData["Parents"] = _context.Categories.Where(a => a.Deleted == 0 && a.ParentId == 0).ToList();
            return View(category);
        }

        // GET: Control/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            ViewData["TemplateId"] = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 2).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture == "en" ? "Name" : "ArName"), category.LangId);
            ViewData["Types"] = new SelectList(_context.Category_Types, "Id", (currentCulture == "en" ? "Title" : "ArTitle"));
            ViewData["Parents"] = _context.Categories.Where(a => a.Deleted == 0 && a.ParentId == 0).ToList();
            return View(category);
        }

        // POST: Control/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArName,Slug,Priority,ParentId,TemplateId,ItemsPerPage,TypeId,Description,ArDescription,LangId,ShowAsMain,ShowInSiteMap,ShowDescription,ShowTitle,ShowThumb,ShowInPath,ShowInSearch,ShowDate,ShowInCatList,Deleted")] Category category)
        {
            if (id != category.Id)
            {
                TempData["error"] = _localizer["Item Not Found"];
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _context.Categories.Attach(category);
                _context.Entry(category).State = EntityState.Modified;
                _context.Entry(category).Property(e => e.UserId).IsModified = false;
                _context.Entry(category).Property(e => e.Thumb).IsModified = false;
                try
                {
                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var ImageUrl = ImagesUplaod.UploadImage(HttpContext, "files/image/CatImages/", _environment.WebRootPath);
                        category.Thumb = ImageUrl.Item1;
                    }

                    category.Name = Functions.RemoveHtml(category.Name);
                    category.ArName = Functions.RemoveHtml(category.ArName);
                    category.Slug = Functions.RemoveHtml(category.Slug);

                    category.Description = Functions.FilterHtml(category.Description);
                    category.ArDescription = Functions.FilterHtml(category.ArDescription);

                    //_context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["success"] = _localizer["Item Updated"].Value;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        TempData["error"] = _localizer["Item Not Found"].Value;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TemplateId"] = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 2).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture == "en" ? "Name" : "ArName"), category.LangId);
            ViewData["Types"] = new SelectList(_context.Category_Types, "Id", (currentCulture == "en" ? "Title" : "ArTitle"));
            ViewData["Parents"] = _context.Categories.Where(a => a.Deleted == 0 && a.ParentId == 0).ToList();
            return View(category);
        }

        // GET: Control/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var category = await _context.Categories
                .Include(c => c.HtmlTemplate)
                .Include(c => c.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // POST: Control/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'DataContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.Deleted = 1;
                _context.Update(category);
                await _context.SaveChangesAsync();
                //_context.Categories.Remove(category);
                TempData["success"] = _localizer["Item Deleted"].Value;
            }
            else
            {
                TempData["error"] = _localizer["Failed To Delete"].Value;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
