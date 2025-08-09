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
    public class ProductCategoriesController : Controller
    {
        private readonly CompanyWebContext _context;

        public ProductCategoriesController(CompanyWebContext context)
        {
            _context = context;
        }

        // GET: ProductCategories
        public IActionResult Index(string keyword, int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var categories = _context.ProductCategories
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



        // GET: ProductCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null) return NotFound();

            return View(productCategory);
        }

        // GET: ProductCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null) return NotFound();

            return View(productCategory);
        }

        // POST: ProductCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] ProductCategory productCategory)
        {
            if (id != productCategory.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null) return NotFound();

            // Kiểm tra nếu đang được dùng bởi sản phẩm
            bool isUsed = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (isUsed)
            {
                ViewBag.ErrorMessage = "Danh mục \"" + productCategory.Title + "\" đang được sử dụng, bạn không thể xoá.";
                return View("DeleteBlocked", productCategory);
            }

            return View(productCategory);
        }

        // POST: Soft delete thay vì xoá vĩnh viễn
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteConfirmed(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                _context.Update(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Trash - Danh sách đã xoá tạm
        public async Task<IActionResult> Trash()
        {
            var deletedCategories = await _context.ProductCategories
                .Where(c => c.IsDeleted == true)
                .ToListAsync();

            return View(deletedCategories);
        }

        // POST: Khôi phục
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return NotFound();

            category.IsDeleted = false;
            _context.Update(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Trash));
        }

        // POST: Xoá vĩnh viễn (nếu muốn)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForever(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category != null)
            {
                _context.ProductCategories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Trash));
        }

        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategories.Any(e => e.Id == id);
        }
    }
}
