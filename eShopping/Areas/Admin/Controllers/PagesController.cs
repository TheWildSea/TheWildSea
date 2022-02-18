using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopping.Infrastructure;
using eShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eShopping.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin, editor")]
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly EShoppingContext _context;

        public PagesController(EShoppingContext context)
        {
            _context = context;
        }

        // GET /admin/Pages
        public async Task<IActionResult> Index()
        {
            // IQueryable<Page> Pages = from p in _context.Pages orderby p.Sorting select p;
            // IQueryable<Page> Pages = _context.Pages.OrderBy(p => p.Sorting);
            // List<Page> PagesList = await Pages.ToListAsync();
            List<Page> PagesList = await _context.Pages.OrderBy(p => p.Sorting).ToListAsync();
            return View(PagesList);
        }

        // GET /admin/Pages/details/5
        public async Task<IActionResult> Details(int id)
        {
            Page page = await _context.Pages.FirstOrDefaultAsync(p => p.Id == id);

            if (page != null)
                return View(page);

            return NotFound();
        }

        // GET /admin/Pages/create
        public IActionResult Create() => View();

        // POST /admin/Pages/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page) // Model binding
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(' ', '-');
                page.Sorting = 100;

                var slug = await _context.Pages.FirstOrDefaultAsync(p => p.Slug == page.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "The title already exists");
                    return View(page); // re-fill the correct information
                }

                _context.Add(page);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";

                return RedirectToAction("Index");
            }

            return View(page);  // re-fill the correct information
        }

        // GET /admin/Pages/edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await _context.Pages.FindAsync(id);

            if (page != null)
                return View(page);

            return NotFound();
        }

        // POST /admin/Pages/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page) // Model binding
        {
            if (ModelState.IsValid)
            {
                // Never bother Homepage
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace(' ', '-');

                // Cannot change it to the same as other existed page
                var slug = await _context.Pages.Where(p => p.Id != page.Id).FirstOrDefaultAsync(p => p.Slug == page.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exists");
                    return View(page); // re-fill the correct information
                }

                _context.Update(page);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The page has been updated!";

                return RedirectToAction("Edit", new { id = page.Id });
            }

            return View(page);  // re-fill the correct information
        }

        // DELETE /admin/Pages/delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await _context.Pages.FindAsync(id);

            if (page == null)
            {
                TempData["Error"] = "The page does not exist!";
            }
            else
            {
                _context.Remove(page);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The page has been deleted!";
            }

            return RedirectToAction("Index");
        }

        // POST /admin/Pages/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1; // the first sorting number because the order of "home" page is zero

            foreach (var pageId in id)
            {
                Page page = await _context.Pages.FindAsync(pageId);
                page.Sorting = count;
                _context.Update(page);
                await _context.SaveChangesAsync();
                ++count;
            }

            return Ok();
        }
    }
}