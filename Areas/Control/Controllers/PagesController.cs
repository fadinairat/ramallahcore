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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Web;

namespace Ramallah.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [Ramallah.AuthorizedAction]
    public class PagesController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public PagesController(DataContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment IHostingEnvironment, IMemoryCache cache, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _environment = IHostingEnvironment;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _localizer = localizer;
        }   

        public async Task<IActionResult> getLinks(string keyword,string type, int lang)
        {
            if (type == "category")
            {
                var catList = await _context.Categories.Where(a => a.Deleted == 0 && (a.LangId == 1 || a.LangId == lang) && (a.Name.Contains(keyword) || a.ArName.Contains(keyword)))
                    .Select(a => new {a.Id, a.Name, a.ArName, a.LangId})
                    .ToListAsync();
                return Json(new
                {
                    kayword = keyword,
                    type = type,
                    lang = lang,
                    links = catList
                });
            }
            else if(type == "file")
            {
                var fileList = await _context.Files.Where(a => a.Deleted == 0 && (a.LangId == 1 || a.LangId == lang) && (a.Name.Contains(keyword) || a.ArName.Contains(keyword)))
                    .Select(a => new {a.Id, a.Name, a.ArName, a.LangId})
                    .ToListAsync();
                return Json(new
                {
                    kayword = keyword,
                    type = type,
                    lang = lang,
                    links = fileList
                });
            }
            else { //Page
                var pagesList = await _context.Pages.Where(a => a.Deleted == false && (a.LangId==1 || a.LangId== lang) && a.Title.Contains(keyword))
                    .Select(a => new { a.Title, a.PageId, a.LangId }).ToListAsync();
                return Json(new
                {
                    kayword = keyword,
                    type = type,
                    lang = lang,
                    links = pagesList
                });
            }
        }

        // GET: Control/Pages
        public async Task<IActionResult> Index(string? keyword, int? scat, int? slang, int PageNumber = 1)
        {
            int PageSize = 20;
            var dataContext = _context.PagesCategories
                .Where(p => p.Page.Deleted == false
                && (keyword == null || p.Page.Title.Contains(keyword))
                && (slang == null || p.Page.LangId == slang)
                && (scat == null || p.Category.Id == scat)
                )
                .Include(p => p.Page)
                .Include(p => p.Page.HtmlTemplate)
                .Include(p => p.Language)
                .Include(p => p.Page.PageRef).Include(p => p.Page.UserAdd)
                .GroupBy(p => p.PageId)
                .Select(x => x.OrderByDescending(y => y.PageId).First())  //This added because GroupBy make a problem when render the query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize);              
                //.OrderByDescending(p => p.); 

            int dbPages = _context.PagesCategories
                .Where(p => p.Page.Deleted == false
                && (keyword == null || p.Page.Title.Contains(keyword))
                && (slang == null || p.Page.LangId == slang)
                && (scat == null || p.Category.Id == scat)
                )
                .GroupBy(p => p.PageId)                
                .Count();

            float paging = (float) dbPages / PageSize;
            double TotalPages = Math.Ceiling(paging);
                
            ViewBag.Categories = _context.Categories.Where(a => a.Deleted == 0).ToList();
            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            ViewBag.Category = scat;
            ViewBag.Language = slang;
            ViewBag.Keyword = keyword;
            ViewBag.PagesCount = TotalPages;
            ViewBag.DBPages = dbPages;
            return View(await dataContext.ToListAsync());
        }

        // GET: Control/Pages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var page = await _context.Pages
                .Include(p => p.HtmlTemplate)
                .Include(p => p.Language)
                .Include(p => p.PageRef)
                .Include(p => p.UserAdd)
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(page);
        }

        // GET: Control/Pages/Create
        public IActionResult Create()
        {
            ViewData["Cats"] = _context.Categories.Where(a => a.Deleted == 0 && a.TypeId == 1).ToList();
            ViewData["Tags"] = _context.Tags.Where(a => a.Deleted == 0).OrderByDescending(a => a.Id).ToList();
            ViewData["TemplateId"] = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type ==1).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture == "en" ? "Name" : "ArName"));
            ViewData["Parent"] = _context.Pages.Where(a => a.Deleted == false && (a.Parent == 0 || a.Parent == null)).ToList();
            ViewData["Forms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 0).ToList();
            return View();
        }

        // POST: Control/Pages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PageId,TranslateId,Title,PageDate,AddDate,LangId,Body,Slug,Url,RedirectUrl,Thumb,Thumb2,ShowThumb,MetaDescription,MetaKeywords,TemplateId,Priority,Publish,Active,AsMenu,ShowAsMain,Parent,ShowInSearchPage,ShowInSiteMap,ShowDate,AllowComment,ShowAsRelated,Summary,ValidDate,SubTitle,Gallery,ShowRelated,Sticky,Deleted,Archive,Views,Video,Audio,UserId,EditedBy,LastEdit,FormId")] Page page, string[] tags_list, string[] cats_list)
        {
            if (ModelState.IsValid)
            {
                page.Title = Functions.RemoveHtml(page.Title);
                page.SubTitle = Functions.RemoveHtml(page.SubTitle);
                page.Url = Functions.RemoveHtml(page.Url);
                page.RedirectUrl = Functions.RemoveHtml(page.RedirectUrl);
                page.Slug = Functions.RemoveHtml(page.Slug);
                page.MetaDescription = Functions.RemoveHtml(page.MetaDescription);
                page.MetaKeywords = Functions.RemoveHtml(page.MetaKeywords);

                page.Summary = Functions.FilterHtml(page.Summary);
                page.Body = Functions.FilterHtml(page.Body);

                if (HttpContext.Session.GetString("id") != "")
                {
                    page.UserId = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                }
                else page.UserId = null;

                page.AddDate = DateTime.Now;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/PageImages/", _environment.WebRootPath,"Thumb");
                    page.Thumb = ImageUrl.Item1;
                }
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/PageImages/", _environment.WebRootPath, "Thumb2");
                    page.Thumb2 = ImageUrl.Item1;
                }

                //Add Category Page record to manay to many table (PagesCategories)
                if (cats_list != null)
                {
                    //Initialize the PageCategories 
                    page.PageCategories = new List<PageCategory>();
                    foreach (var cat in cats_list)
                    {
                        var pg_cat = new PageCategory { PageId = page.PageId, LangId = page.LangId, CategoryId = int.Parse(cat) };
                        page.PageCategories.Add(pg_cat);
                    }                    
                }

                //Save Page to DB
                _context.Add(page);
                await _context.SaveChangesAsync();

                //Put after add page to get the PageId
                if (tags_list != null)
                {
                    foreach (var tag in tags_list)
                    {
                        var TagToAdd = new TagRel { TagId = int.Parse(tag), PageId = page.PageId, LangId = page.LangId };
                        _context.TagsRel.Add(TagToAdd);
                    }
                }
                TempData["success"] = _localizer["Item Added"].Value;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            TempData["error"] = _localizer["Item Not Found"].Value;

            ViewData["TemplateId"] = new SelectList(_context.HtmlTemplates, "Id", "Id", page.TemplateId);
            ViewData["Tags"] = _context.Tags.Where(a => a.Deleted == 0).OrderByDescending(a => a.Id).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", "Id", page.LangId);
            ViewData["Parent"] = new SelectList(_context.Pages, "PageId", "PageId", page.Parent);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", page.UserId);
            ViewData["Forms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 0).ToList();
            return View(page);
        }

        // GET: Control/Pages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            List<PageCategory> pageCats = _context.PagesCategories.Where(i => i.PageId == id)
                .Include(i => i.Category).Where(i => i.PageId == id).ToList();

            

            ViewData["PageCats"] = pageCats;
            ViewData["Cats"] = _context.Categories.Where(a => a.Deleted == 0 && a.TypeId == 1).ToList();
            ViewData["Tags"] = _context.Tags.Where(a => a.Deleted == 0).OrderByDescending(a => a.Id).ToList();
            ViewData["PageTags"] = _context.TagsRel.Where(a => a.PageId == id && a.Deleted == 0).ToList();
            ViewData["TemplateId"] = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 1).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture=="en" ? "Name" : "ArName"), page.LangId);
            ViewData["Parent"] = _context.Pages.Where(a => a.Deleted == false && (a.Parent == 0 || a.Parent == null) && a.PageId != id).ToList();
            ViewData["Forms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 0).ToList();
            return View(page);
        }

        // POST: Control/Pages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PageId,TranslateId,Title,PageDate,AddDate,LangId,Body,Slug,Url,RedirectUrl,ShowThumb,MetaDescription,MetaKeywords,TemplateId,Priority,Publish,Active,AsMenu,ShowAsMain,Parent,ShowInSearchPage,ShowInSiteMap,ShowDate,AllowComment,ShowAsRelated,Summary,ValidDate,SubTitle,Gallery,ShowRelated,Sticky,Deleted,Archive,Views,Video,Audio,UserId,EditedBy,LastEdit,FormId")] Page page, string[] tags_list, string[] cats_list)
        {
            if (id != page.PageId)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            
            if (ModelState.IsValid)
            {
                //Page updatePage = await _context.Pages.FindAsync(id);
                try
                {
                    _context.Pages.Attach(page);
                    _context.Entry(page).State = EntityState.Modified;
                    _context.Entry(page).Property(e => e.AddDate).IsModified = false;
                    _context.Entry(page).Property(e => e.UserId).IsModified = false;
                    _context.Entry(page).Property(e => e.Thumb).IsModified = false;
                    _context.Entry(page).Property(e => e.Thumb2).IsModified = false;

                    page.Title = Functions.RemoveHtml(page.Title);
                    page.SubTitle = Functions.RemoveHtml(page.SubTitle);
                    page.Url = Functions.RemoveHtml(page.Url);
                    page.RedirectUrl = Functions.RemoveHtml(page.RedirectUrl);
                    page.Slug = Functions.RemoveHtml(page.Slug);
                    page.MetaDescription = Functions.RemoveHtml(page.MetaDescription);
                    page.MetaKeywords = Functions.RemoveHtml(page.MetaKeywords);

                    page.Summary = Functions.FilterHtml(page.Summary);
                    page.Body = Functions.FilterHtml(page.Body);
                    

                    List<PageCategory> oldCats = _context.PagesCategories.Where(i => i.PageId == id)
                    .Include(i => i.Category).Where(i => i.PageId == id).ToList();
                    //Add Category Page record to manay to many table (PagesCategories)
                    if (cats_list != null)
                    {
                       
                        foreach (PageCategory pgCat in oldCats)
                        {
                            Boolean removed = true;
                            foreach (var cat in cats_list)
                            {
                                //To check if old category removed from page category list
                                if (pgCat.CategoryId == int.Parse(cat)) removed = false;
                            }
                            if (removed)
                            {
                                //Remove the removed item from the cat list
                                _context.PagesCategories.RemoveRange(_context.PagesCategories.Where(a => a.PageId == id && a.CategoryId == pgCat.CategoryId));
                            }
                        }
                        //Insert the new categories to pagecategory list                   
                        foreach (var cat in cats_list)
                        {
                            //If record not exist then add it
                            if (!_context.PagesCategories.Where(a => a.PageId == id && a.CategoryId == int.Parse(cat)).Any())
                            {
                                var pg_cat = new PageCategory { PageId = page.PageId, LangId = page.LangId, CategoryId = int.Parse(cat) };
                                page.PageCategories.Add(pg_cat);
                            }
                        }
                    }
                    else
                    {
                        _context.PagesCategories.RemoveRange(_context.PagesCategories.Where(a => a.PageId == id));
                    }
                   
                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/PageImages/", _environment.WebRootPath, "Thumb");
                        if(ImageUrl.Item1.ToString() !="") page.Thumb = ImageUrl.Item1;
                    }
                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/PageImages/", _environment.WebRootPath, "Thumb2");
                        if (ImageUrl.Item1.ToString() != "") page.Thumb2 = ImageUrl.Item1;
                    }
                    //Update Tags Many to Many
                    IEnumerable<TagRel> p_tags_list = _context.TagsRel.Where(u => u.PageId == id);
                    _context.TagsRel.RemoveRange(p_tags_list);

                    if (tags_list != null)
                    {
                        foreach (var tag in tags_list)
                        {
                            var TagToAdd = new TagRel { TagId = int.Parse(tag), PageId = id };
                            _context.TagsRel.Add(TagToAdd);
                        }
                    }
                    _context.SaveChanges();

                    TempData["success"] = _localizer["Item Updated"].Value;
                    //_context.Update(updatePage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.PageId))
                    {
                        TempData["error"] = _localizer["Item Not Found"].Value;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            List<PageCategory> pageCats = _context.PagesCategories.Where(i => i.PageId == id)
                .Include(i => i.Category).Where(i => i.PageId == id).ToList();
            ViewData["Tags"] = _context.Tags.Where(a => a.Deleted == 0).OrderByDescending(a => a.Id).ToList();
            ViewData["PageCats"] = pageCats;
            ViewData["Cats"] = _context.Categories.Where(a => a.Deleted == 0 && a.TypeId == 1).ToList();
            ViewData["TemplateId"] = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 1).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture == "en" ? "Name" : "ArName"), page.LangId);
            ViewData["Parent"] = _context.Pages.Where(a => a.Deleted == false && (a.Parent == 0 || a.Parent == null)).ToList();
            ViewData["Forms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 0).ToList();

            return View(page);
        }

        // GET: Control/Pages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var page = await _context.Pages
                .Include(p => p.HtmlTemplate)
                .Include(p => p.Language)
                .Include(p => p.PageRef)
                .Include(p => p.UserAdd)
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(page);
        }

        // POST: Control/Pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pages == null)
            {
                return Problem("Entity set 'DataContext.Pages'  is null.");
            }
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                page.Deleted = true;
                _context.Update(page);
                await _context.SaveChangesAsync();

                TempData["success"] = _localizer["Item Deleted"].Value;
                //_context.Pages.Remove(page);
            }
            else
            {
                TempData["error"] = _localizer["Failed To Delete"].Value;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
          return (_context.Pages?.Any(e => e.PageId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Publish(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var pages = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (pages == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }

            pages.Publish = true;
            _context.Update(pages);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Published"].Value;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnPublish(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var pages = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (pages == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }

            pages.Publish = false;
            _context.Update(pages);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Unpublished"].Value;
            return RedirectToAction("Index");
        }
    }
}
