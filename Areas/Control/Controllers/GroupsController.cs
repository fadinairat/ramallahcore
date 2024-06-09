using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Ramallah.Helpers;
using Ramallah.Models;

namespace Ramallah.Areas.Control.Controllers
{
    [Authorize]
    [Ramallah.AuthorizedAction]
    [Area("Control")]
    public class GroupsController : Controller
    {
        private readonly DataContext _context;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;
        public GroupsController(DataContext context, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _localizer = localizer;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        public async Task<IActionResult> Permissions()
        {
            var permissions = await _context.Permissions.Where(a=> a.Reserved == false).OrderByDescending(x => x.Id).DefaultIfEmpty().ToListAsync();
            return View(permissions);
        }
        // GET: Control/Groups/CreatePermission
        public IActionResult CreatePermission()
        {
            return View();
        }

        // POST: Control/Groups/CreatePermission
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePermission([Bind("Id,Name,Controller,Action,Area,Allowed")] Permissions @perm)
        {
            
            if (ModelState.IsValid)
            {
                if(!_context.Permissions.Where(a => a.Action == perm.Action && a.Controller == perm.Controller && a.Area == perm.Area).Any())
                {
                    perm.UserId = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                    _context.Add(@perm);
                    perm.Reserved = false;
                    await _context.SaveChangesAsync();
                    await _context.SaveChangesAsync();
                    TempData["success"] = _localizer["Item Added"].Value;
                    return RedirectToAction("CreatePermission");
                }
                else
                {
                    TempData["error"] = "Another permission with same details already exists!";
                    return RedirectToAction("Permissions");
                }
                
            }            
            return View(@perm);
        }

        public async Task<IActionResult> EditPermission(int? id)
        {
            if (id == null || _context.Permissions == null)
            {
                TempData["error"] = _localizer["Item Not Found"];
                return RedirectToAction("Permissions");
            }

            var @perm = await _context.Permissions.FindAsync(id);
            if (@perm == null)  
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Permissions");
            }
        
            return View(@perm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPermission(int id, [Bind("Id,Name,Controller,Action,Area,Allowed")] Permissions @perm)
        {            
            if (id != @perm.Id)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Permissions");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_context.Permissions.Where(a => a.Name.Contains(perm.Name) && a.Id != perm.Id).Any())
                    {
                        _context.Permissions.Attach(perm);
                        _context.Entry(perm).State = EntityState.Modified;
                        _context.Entry(perm).Property(e => e.UserId).IsModified = false;
                        _context.Entry(perm).Property(e => e.Reserved).IsModified = false;

                        TempData["success"] = _localizer["Item Updated"].Value;
                        //_context.Update(@group);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        TempData["error"] = "Another permission have the same name...";
                        return RedirectToAction("EditPermission", new { id = id });
                    }
                        
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@perm.Id))
                    {
                        TempData["error"] = _localizer["Item Not Found"].Value;
                        return RedirectToAction("Permissions");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Permissions");
            }
            
            return View(@perm);
        }

        // GET: Control/Groups/DeletePerm/5
        public async Task<IActionResult> DeletePerm(int? id)
        {
            if (id == null || _context.Permissions == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var @perm = await _context.Permissions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@perm == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            var itemsCount = _context.GroupPermissions.Where(a => a.PermissionId == id).Count();
            if (itemsCount > 0)
            {
                TempData["error"] = "Permission assigned to some groups... Remove the assignment first!";
                return RedirectToAction("Permissions");
            }
            return View(@perm);
        }

        // POST: Control/Groups/DeletePerm/5
        [HttpPost, ActionName("DeletePerm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermConfirmed(int id)
        {
            if (_context.Permissions == null)
            {
                return Problem("Entity set 'DataContext.Permissions'  is null.");
            }
            var @perm = await _context.Permissions.FindAsync(id);
            if (@perm != null)
            {
                //perm.Deleted = 1;
                //_context.Permissions.Update(perm);
                _context.Permissions.Remove(@perm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Permissions");
        }

        // GET: Control/Groups
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Groups.Where(a => a.Deleted == 0).Include(a => a.Language);
           
            return View(await dataContext.ToListAsync());
        }

        // GET: Control/Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var @group = await _context.Groups
                .Include(a => a.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(@group);
        }

        // GET: Control/Groups/Create
        public IActionResult Create()
        {
            ViewData["LangId"] = new SelectList(_context.Languages.Where(a => a.Deleted == 0).DefaultIfEmpty(), "Id", "Name");
            ViewData["Permissions"] = _context.Permissions.OrderBy(a => a.Name).DefaultIfEmpty().ToList();
            return View();
        }

        // POST: Control/Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ArName,LangId,P1,P2,P3,P4,P5,P6,P7,P8,P9,P10,P11,P12,Active,Deleted")] Group @group)
        {
            ModelState.Remove("Language");

            if (ModelState.IsValid)
            {
                group.Name = Functions.RemoveHtml(group.Name);
                group.ArName = Functions.RemoveHtml(group.ArName);

                group.UserId = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                _context.Add(@group);
                await _context.SaveChangesAsync();

                //Intialize the Permissions List
                List<Permissions> permissions = _context.Permissions.OrderBy(a => a.Name).ToList();
                group.Permissions = new List<GroupPermissions>();
                foreach (Permissions item in permissions)
                {
                    if (HttpContext.Request.Form["p_" + item.Id] == "on")
                    {
                        var group_perm = new GroupPermissions { GroupId = group.Id, PermissionId = item.Id };
                        _context.GroupPermissions.Add(group_perm);
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["LangId"] = new SelectList(_context.Languages.Where(a => a.Deleted == 0), "Id", "Name", @group.LangId);
            return View(@group);
        }

        // GET: Control/Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            ViewData["Permissions"] = _context.Permissions.Where(a => a.Reserved == false).OrderBy(a => a.Controller).ToList();
            ViewData["GroupPermissions"] = _context.GroupPermissions.Where(a => a.GroupId == id).ToList();
            ViewData["LangId"] = new SelectList(_context.Languages.Where(a => a.Deleted == 0), "Id", "Name", @group.LangId);
           
            return View(@group);
        }

        // POST: Control/Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArName,LangId,P1,P2,P3,P4,P5,P6,P7,P8,P9,P10,P11,P12,Active,Deleted")] Group @group)
        {
            ModelState.Remove("Language");
            if (id != @group.Id)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Groups.Attach(group);
                    _context.Entry(group).State = EntityState.Modified;
                    _context.Entry(group).Property(e => e.UserId).IsModified = false;

                    group.Name = Functions.RemoveHtml(group.Name);
                    group.ArName = Functions.RemoveHtml(group.ArName);
                    //Intialize the Permissions List
                    List<Permissions> permissions = _context.Permissions.OrderBy(a => a.Name).ToList();
                    //group.Permissions = new List<GroupPermissions>();
                    foreach (Permissions item in permissions)
                    {
                        if (HttpContext.Request.Form["p_" + item.Id] == "on")
                        {
                            if(!_context.GroupPermissions.Where(a => a.PermissionId== item.Id && a.GroupId == id).Any())
                            {
                                var group_perm = new GroupPermissions { GroupId = group.Id, PermissionId = item.Id };
                                _context.GroupPermissions.Add(group_perm);
                            }
                        }
                        else
                        {
                            _context.GroupPermissions.RemoveRange(_context.GroupPermissions.Where(a => a.GroupId == id && a.PermissionId == item.Id));
                        }
                    }

                    //_context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
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
            ViewData["LangId"] = new SelectList(_context.Languages.Where(a => a.Deleted == 0), "Id", "Name", @group.LangId);
            return View(@group);
        }

        // GET: Control/Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }
            
            var @group = await _context.Groups
                .Include(a => a.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("Index");
            }

            return View(@group);
        }

        // POST: Control/Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Groups == null)
            {
                return Problem("Entity set 'DataContext.Groups'  is null.");
            }
            var @group = await _context.Groups.FindAsync(id);
            if (@group != null)
            {
                group.Deleted = 1;
                _context.Groups.Update(group);
                //_context.Groups.Remove(@group);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
          return (_context.Groups?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
