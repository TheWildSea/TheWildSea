using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopping.Infrastructure;
using eShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eShopping.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly EShoppingContext _context;
        public ProductsController(EShoppingContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(int p = 1)
        {
            const int Pagesize = 6;
            var products = await _context.Products.OrderByDescending(p => p.Id)
                                                  .Skip((p - 1) * Pagesize)
                                                  .Take(Pagesize)
                                                  .ToListAsync();

            const int pageRange = 10;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageRange;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.Products.Count() / Pagesize);

            return View(products);
        }

        
        public async Task<IActionResult> ProductsByCategory(string categorySlug, int p = 1)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == categorySlug);
            if (category == null) return RedirectToAction("Index");

            const int Pagesize = 6;
            var products = await _context.Products.OrderByDescending(p => p.Id)
                                                  .Where(p => p.CategoryId == category.Id)
                                                  .Skip((p - 1) * Pagesize)
                                                  .Take(Pagesize)
                                                  .ToListAsync();

            const int pageRange = 10;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageRange;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.Products.Where(p => p.CategoryId == category.Id).Count() / Pagesize);

            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug = category.Slug;

            return View(products);
        }
    }
}