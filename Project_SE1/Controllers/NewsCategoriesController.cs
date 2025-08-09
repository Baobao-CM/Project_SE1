using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_dnc_se1.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace project_dnc_se1.Controllers
{
    public class NewsCategoriesController : Controller
    {
        private readonly CompanyWebContext _context;

        public NewsCategoriesController(CompanyWebContext context)
        {
            _context = context;
        }

        // GET: NewsCategories
        public IActionResult Index(string keyword, int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var categories = _context.NewsCategories
                .Where(c => c.IsDeleted != true)  // fix nullable bool here
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                categories = categories.Where(c => c.Title.Contains(keyword));
            }

            var pagedList = categories
                .OrderBy(c => c.Id)
                .ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        // GET: NewsCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsCategory = await _context.NewsCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsCategory == null)
            {
                return NotFound();
            }

            return View(newsCategory);
        }

        // GET: NewsCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewsCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] NewsCategory newsCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newsCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsCategory);
        }

        // GET: NewsCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsCategory = await _context.NewsCategories.FindAsync(id);
            if (newsCategory == null)
            {
                return NotFound();
            }
            return View(newsCategory);
        }

        // POST: NewsCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] NewsCategory newsCategory)
        {
            if (id != newsCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsCategoryExists(newsCategory.Id))
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
            return View(newsCategory);
        }

        // Soft delete
        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var nc = await _context.NewsCategories.FindAsync(id);
            if (nc == null)
            {
                return NotFound();
            }

            nc.IsDeleted = true;
            _context.Update(nc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Restore từ trash
        [HttpPost, ActionName("Restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var nc = await _context.NewsCategories.FindAsync(id);
            if (nc == null)
            {
                return NotFound();
            }

            nc.IsDeleted = false;
            _context.Update(nc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Trash));
        }

        // Trang danh sách đã xoá
        public async Task<IActionResult> Trash()
        {
            var deletedNews = await _context.NewsCategories
                .Where(n => n.IsDeleted == true)
                .ToListAsync();

            return View(deletedNews);
        }

        // POST: NewsCategories/Delete/5 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newsCategory = await _context.NewsCategories.FindAsync(id);
            if (newsCategory != null)
            {
                _context.NewsCategories.Remove(newsCategory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Trash));
        }

        private bool NewsCategoryExists(int id)
        {
            return _context.NewsCategories.Any(e => e.Id == id);
        }
    }
}
