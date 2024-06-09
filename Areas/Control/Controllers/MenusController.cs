using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ramallah.Helpers;
using Ramallah.Models;
using Microsoft.Extensions.Localization;

namespace Ramallah.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [Ramallah.AuthorizedAction]
    public class MenusController : Controller
    {
        private readonly DataContext _context;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public MenusController(DataContext context, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _localizer = localizer;
        }

        public async Task<IActionResult> getMenuParents(int langId, int location)
        {
            List<Menu> menuList = await _context.Menus.Where(a => a.Deleted == 0 && a.LocationId == location && (a.ParentId == 0 || a.ParentId == null) && (a.LangId == langId || a.LangId == 1)).ToListAsync();
            //.Where(a => a.Deleted == 0 && a.LocationId == location && (a.ParentId == 0 || a.ParentId==null) && (a.LangId == langId && a.LangId==1))
            return Json(new
            {
                lang = langId,
                location = location,
                menus = menuList
            });
        }

        // GET: Control/Menus
        public async Task<IActionResult> Index(string? keyword, int? slang, int? slocation)
        {
            var menus = await _context.Menus.Where(a => a.Deleted == 0 && (a.ParentId == 0 || a.ParentId == null)
                && (slocation == null || a.LocationId == slocation)
                && (slang == null || a.LangId == slang)
                && (keyword == null || a.Name.Contains(keyword))
                )
                .Include(a => a.MenuParentRef)
                .Include(a => a.MenuLocation)
                .Include(a => a.Language)
                .Include(a => a.User)
                .ToListAsync();

            var submenus = await _context.Menus.Where(a => a.Deleted == 0 && a.ParentId != 0 && a.ParentId != null)
                .Include(a => a.MenuParentRef)
                .Include(a => a.MenuLocation)
                .Include(a => a.Language)
                .Include(a => a.User)
                .ToListAsync();

            ViewBag.Locations = _context.MenuLocations.OrderBy(a => a.Id).ToList();
            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            ViewBag.SubMenus = submenus;

            ViewBag.Keyword = keyword;
            ViewBag.Language = slang;
            ViewBag.Location = slocation;
            return View(menus);
        }

        // GET: Control/Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var menu = await _context.Menus
                .Include(a => a.MenuLocation)
                .Include(a => a.Language)
                .Include(a => a.User)
                .Include(a => a.MenuParentRef)
            .FirstOrDefaultAsync(m => m.Id == id);


            return View(menu);
        }

        // GET: Control/Menus/Create
        public IActionResult Create()
        {
            ViewBag.Parents = _context.Menus.Where(a => (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0)
                .OrderBy(a => a.Id).ToList();
            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            List<MenuLocation> locations = _context.MenuLocations.OrderBy(a => a.Id).ToList();
            ViewBag.Locations = locations;
            return View();
        }

        // POST: Control/Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Menu menu)
        public async Task<IActionResult> Create([Bind("Id,Name,LocationId,UserId,Target,ParentId,Priority,Link,LangId")] Menu menu)
        {
            ModelState.Remove("MenuParentRef");
            ModelState.Remove("ParentMenus");
            if (ModelState.IsValid)
            {
                menu.Name = Functions.RemoveHtml(menu.Name);
                menu.Target = Functions.RemoveHtml(menu.Target);
                menu.Link = Functions.RemoveHtml(menu.Link);

                menu.UserId = int.Parse(HttpContext.Session.GetString("id") ?? "1");

                await _context.AddAsync(menu);
                await _context.SaveChangesAsync();

                TempData["success"] = _localizer["Item Updated"].Value;
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Create");
            }

            TempData["error"] = "Cannot add menu...";
            List<MenuLocation> locations = _context.MenuLocations.OrderBy(a => a.Id).ToList();
            ViewBag.Locations = locations;
            ViewBag.Parents = _context.Menus.Where(a => (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0)
                .OrderBy(a => a.Id).ToList();
            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();

            return View(menu);
        }

        // GET: Control/Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            ViewBag.Parents = _context.Menus.Where(a => (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0)
                .OrderBy(a => a.Id).ToList();
            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            ViewBag.Locations = _context.MenuLocations.OrderBy(a => a.Id).ToList();

            return View(menu);
        }

        // POST: Control/Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LocationId,Target,ParentId,Priority,Link,LangId,Active,Deleted")] Menu menu)
        {
            ModelState.Remove("MenuParentRef");
            ModelState.Remove("ParentMenus");
            if (id != menu.Id)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Menus.Attach(menu);
                    _context.Entry(menu).State = EntityState.Modified;
                    _context.Entry(menu).Property(p => p.UserId).IsModified = false;
                    _context.Entry(menu).Reference(p => p.User).IsModified = false;

                    menu.Name = Functions.RemoveHtml(menu.Name);
                    menu.Target = Functions.RemoveHtml(menu.Target);
                    menu.Link = Functions.RemoveHtml(menu.Link);

                    //_context.Update(menu);
                    await _context.SaveChangesAsync();
                    TempData["success"] = _localizer["Item Updated"].Value;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
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
            ViewBag.Parents = _context.Menus.Where(a => (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0)
                .OrderBy(a => a.Id).ToList();
            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            List<MenuLocation> locations = _context.MenuLocations.OrderBy(a => a.Id).ToList();
            ViewBag.Locations = locations;

            return View(menu);
        }

        // GET: Control/Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var menu = await _context.Menus
                .Include(a => a.MenuLocation)
                .Include(a => a.Language)
                .Include(a => a.User)
                .Include(a => a.MenuParentRef)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(menu);
        }

        // POST: Control/Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Menus == null)
            {
                return Problem("Entity set 'DataContext.Menus'  is null.");
            }
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                TempData["success"] = _localizer["Item Deleted"].Value;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return (_context.Menus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
