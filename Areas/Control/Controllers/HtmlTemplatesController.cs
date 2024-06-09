using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ramallah.Models;
using Ramallah.Helpers;
using Microsoft.Extensions.Localization;

namespace Ramallah.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [Ramallah.AuthorizedAction]
    public class HtmlTemplatesController : Controller
    {
        private readonly DataContext _context;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HtmlTemplatesController(DataContext context, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _localizer = localizer;
        }

        // GET: Control/HtmlTemplates
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.HtmlTemplates.Where(a => a.Deleted == 0).Include(h => h.HtmlTemplatesType).Include(h => h.Language).Include(h => h.User);
            return View(await dataContext.ToListAsync());
        }

        // GET: Control/HtmlTemplates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HtmlTemplates == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var htmlTemplate = await _context.HtmlTemplates
                .Include(h => h.HtmlTemplatesType)
                .Include(h => h.Language)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (htmlTemplate == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(htmlTemplate);
        }

        // GET: Control/HtmlTemplates/Create
        public IActionResult Create()
        {
            ViewData["Type"] = new SelectList(_context.HtmlTemplatesTypes, "Id", "Title");
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Nickname");
            return View();
        }

        // POST: Control/HtmlTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ArName,Type,FilePath,LangId")] HtmlTemplate htmlTemplate)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("id") != "")
                {
                    htmlTemplate.UserId = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                }
                else htmlTemplate.UserId = null;

                htmlTemplate.Name = Functions.RemoveHtml(htmlTemplate.Name);
                htmlTemplate.ArName = Functions.RemoveHtml(htmlTemplate.ArName);

                _context.Add(htmlTemplate);
                await _context.SaveChangesAsync();
                TempData["success"] = _localizer["Item Added"].Value;
                return RedirectToAction(nameof(Index));
            }
            ViewData["Type"] = new SelectList(_context.HtmlTemplatesTypes, "Id", "Title", htmlTemplate.Type);
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", "Name", htmlTemplate.LangId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Nickname", htmlTemplate.UserId);
            return View(htmlTemplate);
        }

        // GET: Control/HtmlTemplates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HtmlTemplates == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var htmlTemplate = await _context.HtmlTemplates.FindAsync(id);
            if (htmlTemplate == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            ViewData["Type"] = new SelectList(_context.HtmlTemplatesTypes, "Id", "Title", htmlTemplate.Type);
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", "Name", htmlTemplate.LangId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Nickname", htmlTemplate.UserId);
            return View(htmlTemplate);
        }

        // POST: Control/HtmlTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArName,Type,FilePath,LangId")] HtmlTemplate htmlTemplate)
        {
            if (id != htmlTemplate.Id)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    htmlTemplate.Name = Functions.RemoveHtml(htmlTemplate.Name);
                    htmlTemplate.ArName = Functions.RemoveHtml(htmlTemplate.ArName);

                    _context.Update(htmlTemplate);
                    await _context.SaveChangesAsync();
                    TempData["success"] = _localizer["Item Updated"].Value;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HtmlTemplateExists(htmlTemplate.Id))
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
            TempData["error"] = "Cannot edit the template";

            ViewData["Type"] = new SelectList(_context.HtmlTemplatesTypes, "Id", "Title", htmlTemplate.Type);
            ViewData["LangId"] = new SelectList(_context.Languages, "Id", "Name", htmlTemplate.LangId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Nickname", htmlTemplate.UserId);
            return View(htmlTemplate);
        }

        // GET: Control/HtmlTemplates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HtmlTemplates == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var htmlTemplate = await _context.HtmlTemplates
                .Include(h => h.HtmlTemplatesType)
                .Include(h => h.Language)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (htmlTemplate == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(htmlTemplate);
        }

        // POST: Control/HtmlTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HtmlTemplates == null)
            {
                return Problem("Entity set 'DataContext.HtmlTemplates'  is null.");
            }
            var htmlTemplate = await _context.HtmlTemplates.FindAsync(id);
            if (htmlTemplate != null)
            {
                htmlTemplate.Deleted = 1;
                _context.Update(htmlTemplate);
                await _context.SaveChangesAsync();

                TempData["success"] = _localizer["Item Deleted"].Value;
                //_context.HtmlTemplates.Remove(htmlTemplate);
            }
            else
            {
                TempData["error"] = "Cannot remove the template...";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HtmlTemplateExists(int id)
        {
          return (_context.HtmlTemplates?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
