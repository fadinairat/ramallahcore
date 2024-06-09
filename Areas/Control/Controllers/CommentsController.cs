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

namespace Ramallah.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [Ramallah.AuthorizedAction]
    public class CommentsController : Controller
    {
        private readonly DataContext _context;

        public CommentsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> getCommentDetails(int id)
        {
            if (id == null || _context.Comments == null)
            {
                return Json(new
                {
                    Success = false,
                    Msg = "Comment not found"
                }) ;
            }

            Comments comm = await _context.Comments.FindAsync(id);
            return Json(new
            {
                Success = true,
                Details = comm,
                Msg = ""
            });
        }
        // GET: Control/Comments
        public async Task<IActionResult> Index(string? keyword)
        {
            var comm = _context.Comments.Where(a => a.Deleted== false &&
            (keyword == null || a.Name.Contains(keyword) || a.Location.Contains(keyword) || a.Email.Contains(keyword) || a.Subject.Contains(keyword)));
            ViewBag.keyword = keyword;

            return _context.Comments != null ?
                        View(comm) :
                        Problem("Entity set 'MemContext.Comments'  is null.");
        }

        // GET: Control/Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // GET: Control/Comments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Control/Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Location,Subject,Body,Published,Reviewed,AddTime,Deleted")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                comments.Subject = Functions.RemoveHtml(comments.Subject);
                comments.Body = Functions.RemoveHtml(comments.Body);
                comments.Email = Functions.RemoveHtml(comments.Email);

                _context.Add(comments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(comments);
        }

        // GET: Control/Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments.FindAsync(id);
            if (comments == null)
            {
                return NotFound();
            }
            return View(comments);
        }

        // POST: Control/Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Location,Subject,Body,Published,Reviewed,AddTime,Deleted")] Comments comments)
        {
            if (id != comments.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comments.Subject = Functions.RemoveHtml(comments.Subject);
                    comments.Body = Functions.RemoveHtml(comments.Body);
                    comments.Email = Functions.RemoveHtml(comments.Email);
                    _context.Update(comments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentsExists(comments.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(comments);
        }

        // GET: Control/Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // POST: Control/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                TempData["error"] = "Comment not found...";
                return Problem("Entity set 'MemContext.Comments'  is null.");
            }
            var comments = await _context.Comments.FindAsync(id);
            if (comments != null)
            {
                comments.Deleted = true;
                _context.Comments.Update(comments);
                TempData["success"] = "Comment deleted successfully...";
                //_context.Comments.Remove(comments);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentsExists(int id)
        {
            return (_context.Comments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
