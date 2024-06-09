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
    public class FilesController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public FilesController(DataContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment IHostingEnvironment, IMemoryCache cache, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _environment = IHostingEnvironment;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _localizer = localizer;
        }

        public async Task<IActionResult> getFilesCategories(int type)
        {
            if (type == null)
            {
                return Json(new
                {
                    success = false,
                    type = type
                });
            }

            List<Category> catList = await _context.Categories.Where(a => a.Deleted == 0 && a.TypeId == type).ToListAsync();
            return Json(new
            {
                success = true,
                type = type,
                cats = catList
            });
        }

        //[HttpGet("download/{*path}")]
        public IActionResult Getfile(string path)
        {
            //var allowedDirectory = "/files/file";
            //var fullPath = Path.Combine(allowedDirectory, path);
            //!fullPath.StartsWith(allowedDirectory) || 
           
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", Path.GetFileName(path));

        }

        // GET: Control/Files
        public async Task<IActionResult> Index(string? keyword, int? stype, int? slang,int? scat, int PageNumber = 1)
        {
            int PageSize = 20;

            var dataContext = _context.Files.Where(a => a.Deleted == 0
                && (keyword == null || a.Name.Contains(keyword) || a.ArName.Contains(keyword))
                && (stype == null || a.Category.TypeId == stype)
                && (slang == null || a.LangId == slang)
                && (scat == null || a.CatId == scat)
                )
                .Include(f => f.Category)
                .Include(f => f.Category.CategoryTypes)
                .Include(f => f.Language)
                .Include(f => f.User)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .OrderByDescending(a => a.Id);

            int dbPages = _context.Files.Where(a => a.Deleted == 0
                && (keyword == null || a.Name.Contains(keyword) || a.ArName.Contains(keyword))
                && (stype == null || a.Category.TypeId == stype)
                && (slang == null || a.LangId == slang)
                && (scat == null || a.CatId == scat)
                )
                .Include(f => f.Category)
                .Include(f => f.Language)
                .Include(f => f.User)
                .Count();

            float paging = (float)dbPages / PageSize;
            double TotalPages = Math.Ceiling(paging);

            ViewBag.Types = _context.Category_Types.Where(a=> a.Id >= 2 && a.Id <= 5).ToList();
            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            ViewBag.Categories = _context.Categories.Where(a => a.Deleted == 0).ToList();
            ViewBag.Type = stype;
            ViewBag.Language = slang;
            ViewBag.Keyword = keyword;
            ViewBag.Category = scat;
            ViewBag.PagesCount = TotalPages;
            return View(await dataContext.ToListAsync());
        }

        // GET: Control/Files/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Files == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var files = await _context.Files
                .Include(f => f.Category)
                .Include(f => f.Language)
                .Include(f => f.Category.CategoryTypes)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (files == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(files);
        }

        // GET: Control/Files/Create
        public IActionResult Create()
        {
            
            ViewData["Cats"] = _context.Categories.Where(a => a.Deleted == 0 && a.TypeId == 2).ToList();
            ViewData["Types"] = _context.Category_Types.Where(a => a.Id >= 2 && a.Id <= 5).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture=="en" ? "Name" : "ArName"));
            
            return View();
        }

        // POST: Control/Files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CatId,Name,ArName,Year,Parent,Publish,Active,Thumb,LangId,Description,ArDescription,FilePath,Source,Priority,ShowHome,AllowComment,Date,Deleted")] Files files)
        {
            if (ModelState.IsValid)
            {
                
                files.AddDate = DateTime.Now;
                files.UserId = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/", _environment.WebRootPath, "FilePath");
                    files.FilePath = filePath;
                }
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/FilesImages/", _environment.WebRootPath, "Thumb");
                    files.Thumb = ImageUrl.Item1;
                }
                files.ArName = Functions.RemoveHtml(files.ArName);
                files.Name = Functions.RemoveHtml(files.Name);
                files.FilePath = Functions.RemoveHtml(files.FilePath);

                files.Description = Functions.FilterHtml(files.Description);
                files.ArDescription = Functions.RemoveHtml(files.ArDescription);

                _context.Add(files);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cats"] = _context.Categories.Where(a => a.Deleted == 0 && a.TypeId == 2).ToList();
            ViewData["Types"] = _context.Category_Types.Where(a => a.Id >= 2 && a.Id <= 5).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture=="en" ? "Name" : "ArName"), files.LangId);
            return View(files);
        }

        // GET: Control/Files/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Files == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var files = await _context.Files.Where(a => a.Id == id).Include(a => a.Category.CategoryTypes).FirstOrDefaultAsync();
            if (files == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            ViewData["Cats"] = _context.Categories.Where(a => a.Deleted == 0 && a.TypeId == files.Category.CategoryTypes.Id).ToList();
            ViewData["Types"] = _context.Category_Types.Where(a => a.Id >= 2 && a.Id <= 5).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture=="en" ? "Name" : "ArName"), files.LangId);
            return View(files);
        }

        // POST: Control/Files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CatId,Name,ArName,Year,Parent,Publish,Active,Thumb,LangId,Description,ArDescription,FilePath,Source,Priority,ShowHome,AllowComment,UserId,Date,Deleted")] Files files)
        {
            if (id != files.Id)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Files.Attach(files);
                    _context.Entry(files).State = EntityState.Modified;
                    _context.Entry(files).Property(e => e.UserId).IsModified = false;
                    _context.Entry(files).Property(e => e.AddDate).IsModified = false;
                    _context.Entry(files).Property(e => e.FilePath).IsModified = false;
                    _context.Entry(files).Property(e => e.Thumb).IsModified = false;
                    files.UpdatedAt = DateTime.Now;

                    files.ArName = Functions.RemoveHtml(files.ArName);
                    files.Name = Functions.RemoveHtml(files.Name);
                    files.FilePath = Functions.RemoveHtml(files.FilePath);

                    files.Description = Functions.FilterHtml(files.Description);
                    files.ArDescription = Functions.RemoveHtml(files.ArDescription);

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/", _environment.WebRootPath, "FilePath");
                        files.FilePath = filePath;
                    }
                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/FilesImages/", _environment.WebRootPath, "Thumb");
                        files.Thumb = ImageUrl.Item1;
                    }
                    TempData["success"] = _localizer["Item Updated"].Value;// "File updated successfully...";
                    //_context.Update(files);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilesExists(files.Id))
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
            ViewData["Cats"] = _context.Categories.Where(a => a.Deleted == 0 && a.TypeId == 2).ToList();
            ViewData["Types"] = _context.Category_Types.Where(a => a.Id >= 2 && a.Id <= 5).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", (currentCulture=="en" ? "Name" : "ArName"), files.LangId);
            return View(files);
        }

        // GET: Control/Files/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Files == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var files = await _context.Files
                .Include(f => f.Category)
                .Include(f => f.Language)
                .Include(f => f.Category.CategoryTypes)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (files == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(files);
        }

        // POST: Control/Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Files == null)
            {
                return Problem("Entity set 'DataContext.Files'  is null.");
            }
            var files = await _context.Files.FindAsync(id);
            if (files != null)
            {
                files.Deleted = 1;
                _context.Update(files);
                await _context.SaveChangesAsync();

                TempData["success"] = _localizer["Item Deleted"].Value;
                //_context.Files.Remove(files);
            }
            else
            {
                TempData["error"] = "Cannot remove the page...";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilesExists(int id)
        {
          return (_context.Files?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
