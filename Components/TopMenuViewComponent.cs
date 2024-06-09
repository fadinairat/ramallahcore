using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ramallah.Models;

namespace Ramallah.Components
{
    public class TopMenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public TopMenuViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Menu> ParentMenus = _context.Menus.Where(a => a.LocationId == 5 && (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0 && a.Active == 1).OrderBy(a => a.Priority).ThenBy(a => a.Id)
                .Include(a => a.MenuLocation)
                .ToList();

            List<Menu> SubMenus = _context.Menus.Where(a => a.LocationId == 5 && a.ParentId != 0 && a.ParentId != null && a.Deleted == 0 && a.Active == 1).OrderBy(a => a.Priority).OrderBy(a => a.Id).ToList();

            ViewBag.ParentMenus = ParentMenus;
            ViewBag.SubMenus = SubMenus;

            ViewBag.FastNews = _context.PagesCategories
            .Include(a => a.Page)
            .Include(a => a.Category)
            .Where(a => a.CategoryId == 18 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
            .OrderByDescending(a => a.Page.PageDate)
            .OrderByDescending(a => a.Page.PageId)
            .Take(10)
            .ToList();


            return View("Default");
        }
    }
}
