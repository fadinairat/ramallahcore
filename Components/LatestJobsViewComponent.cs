using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ramallah.Models;

namespace Ramallah.Components
{
    public class LatestJobsViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public LatestJobsViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            return View("Default");
        }
    }
}
