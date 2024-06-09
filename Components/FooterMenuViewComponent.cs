using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ramallah.Models;
using System.Data;
using System.Text.Json;
using Newtonsoft.Json;

namespace Ramallah.Components
{
    public class FooterMenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public FooterMenuViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Menu> MunMenus = _context.Menus.Where(a => a.LocationId == 5 && (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0 && a.Active == 1).OrderByDescending(a => a.Priority).OrderBy(a => a.Id)
                .Include(a => a.MenuLocation)
                .ToList();
            List<Menu> CityMenus = _context.Menus.Where(a => a.LocationId == 6 && (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0 && a.Active == 1).OrderByDescending(a => a.Priority).OrderBy(a => a.Id)
                .Include(a => a.MenuLocation)
                .ToList();


            var footerDesc = _context.Pages.Where(a => a.Deleted == false && a.Active == true && a.Title.Contains("تواصل معنا")).FirstOrDefault();
            var footerContact = _context.Pages.Where(a => a.Deleted == false && a.Active == true && a.Title.Contains("اتصل بنا - سفلي")).FirstOrDefault();

            ViewBag.FooterContact = footerContact;
            ViewBag.FooterDesc = footerDesc;
            ViewBag.MunMenus = MunMenus;
            ViewBag.CityMenus = CityMenus;


            var visits = _context.Visits.FirstOrDefault();
            if (visits != null)
            {
                ViewBag.Visits = visits.VisitsCount;
                visits.VisitsCount = visits.VisitsCount + 1;
                _context.Visits.Update(visits);
                _context.SaveChanges();
            }

            return View("Default");
        }
    }
}