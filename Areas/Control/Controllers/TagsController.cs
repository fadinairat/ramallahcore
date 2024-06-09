using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ramallah.Models;
using Ramallah.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System.Web;
using Microsoft.Extensions.Localization;

namespace Ramallah.Areas.Control.Controllers
{
    //[Authorize(Roles = "Editor")]
    [Authorize]
    [Ramallah.AuthorizedAction]
    [Area("Control")]
    [BindProperties]
    public class TagsController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;
        public TagsController(DataContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment IHostingEnvironment, IMemoryCache cache, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _environment = IHostingEnvironment;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _localizer = localizer;
        }

        // GET: Control/Tags
        public async Task<IActionResult> Index()
        {
            var tag = await _context.Tags.Where(a => a.Deleted == 0)
                .Include(a => a.Language)
                .Include(a => a.User)
                .Include(a => a.HtmlTemplate)
                .ToListAsync();
            return tag != null ? 
                          View(tag) :
                          Problem("Entity set 'DataContext.Tags'  is null.");
        }

        // GET: Control/Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(tag);
        }

        // GET: Control/Tags/Create
        public IActionResult Create()
        {
            //IEnumerable<HtmlTemplate> Templates = _context.HtmlTemplates.Where(a => a.Deleted == 0);
            ViewBag.Templates = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 3).ToList();
            //ViewBag.Templates = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 3).ToList();
            //Templates = new SelectList(_context.HtmlTemplates.Where(a => a.Deleted == 0), "Id", "Name");

            return View();
        }

        // POST: Control/Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Url,TempId,ArName,Thumb,UserId,ItemsPerPage,Deleted")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                tag.Name = Functions.RemoveHtml(tag.Name);
                tag.ArName = Functions.RemoveHtml(tag.ArName);
                tag.UserId = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                _context.Add(tag);

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadImage(HttpContext, "files/image/TagImages/", _environment.WebRootPath);
                    tag.Thumb = ImageUrl.Item1;
                }

                await _context.SaveChangesAsync();
                TempData["success"] = _localizer["Item Added"].Value;
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Templates = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 3).ToList();
            return View(tag);
        }

        // GET: Control/Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            ViewBag.Templates = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type==3).ToList();

            return View(tag);
        }

        // POST: Control/Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url,TempId,ArName,Thumb,UserId,ItemsPerPage,Deleted")] Tag tag)
        {
            if (id != tag.Id)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var excluded = new[] { "Thumb"};
                    //var entry = _context.Entry(tag);
                    //entry.State = EntityState.Modified;
                    //foreach (var name in excluded)
                    //{
                    //    entry.Property(name).IsModified = false;
                    //}
                    tag.Name = Functions.RemoveHtml(tag.Name);
                    tag.ArName = Functions.RemoveHtml(tag.ArName);

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var ImageUrl = ImagesUplaod.UploadImage(HttpContext, "files/image/TagImages/", _environment.WebRootPath);
                        if (ImageUrl.Item1 != "") tag.Thumb = ImageUrl.Item1;
                    }

                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.Id))
                    {
                        TempData["error"] = _localizer["Item Not Found"].Value;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = _localizer["Item Updated"].Value;
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Templates = _context.HtmlTemplates.Where(a => a.Deleted == 0 && a.Type == 3).ToList();
            TempData["error"] = "Cannot update the tag...";
            return View(tag);
        }

        // GET: Control/Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(tag);
        }

        // POST: Control/Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tags == null)
            {
                return Problem("Entity set 'DataContext.Tags'  is null.");
            }
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                TempData["success"] = _localizer["Item Deleted"].Value;
                _context.Tags.Remove(tag);
            }
            else
            {
                TempData["error"] = "Failed to delete tag...";
            }
            
            await _context.SaveChangesAsync();            
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
          return (_context.Tags?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
