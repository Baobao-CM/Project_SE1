using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project_dnc_se1.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace project_dnc_se1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CompanyWebContext _context;

        public ProductsController(CompanyWebContext context)
        {
            _context = context;
        }

        // GET: Products (Phân trang + tìm kiếm)
        public IActionResult Index(string keyword, int? page, int? categoryId)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            ViewBag.Categories = _context.ProductCategories.ToList();
            ViewBag.SelectedCategoryId = categoryId;

            var products = _context.Products
                                   .Include(p => p.Category)
                                   .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                products = products.Where(p => p.Title.Contains(keyword));

            if (categoryId.HasValue && categoryId > 0)
                products = products.Where(p => p.CategoryId == categoryId);

            var pagedList = products.OrderBy(p => p.Id)
                                    .ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Title");
            return View();
        }

        // POST: Products/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Title");

                return View();
            }

            // Upload các ảnh chi tiết
            var imageUrls = new List<string>();
            if (product.ImageFiles?.Count > 0)
            {
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                Directory.CreateDirectory(uploadDir);

                foreach (var file in product.ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var path = Path.Combine(uploadDir, fileName);
                        using var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        imageUrls.Add("/images/products/" + fileName);
                    }
                }
                product.ImageUrls = string.Join(";", imageUrls);
            }

            // Upload ảnh thumbnail
            if (product.ThumbNailFile?.Length > 0)
            {
                var thumbDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/thumbnails");
                Directory.CreateDirectory(thumbDir);

                var thumbName = $"{Guid.NewGuid()}{Path.GetExtension(product.ThumbNailFile.FileName)}";
                var thumbPath = Path.Combine(thumbDir, thumbName);
                using var thumbStream = new FileStream(thumbPath, FileMode.Create);
                await product.ThumbNailFile.CopyToAsync(thumbStream);
                product.ThumbNail = "/images/thumbnails/" + thumbName;
            }

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Title", product.CategoryId);

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Product product,
            List<IFormFile>? productImages,
            IFormFile? thumbnailFile,
            string? removedImages)
        {
            if (id != product.Id) return NotFound();

            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return NotFound();

            // Nếu ModelState không hợp lệ thì giữ ViewData để render lại
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Title", product.CategoryId);
                return View(product);
            }

            // Cập nhật các trường cơ bản
            existing.Title = product.Title;
            existing.Description = product.Description;
            existing.CategoryId = product.CategoryId;
            existing.IsFeatured = product.IsFeatured;
            existing.UpdatedAt = DateTime.Now;

            // ==========================
            // XỬ LÝ ẢNH CHI TIẾT
            // ==========================
            var currentImages = existing.ImageUrls?
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .ToList() ?? new List<string>();

            // Xóa ảnh nếu có chỉ định
            if (!string.IsNullOrEmpty(removedImages))
            {
                foreach (var img in removedImages.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                    currentImages.Remove(img);
                }
            }

            // Thêm ảnh mới
            if (productImages?.Count > 0)
            {
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                Directory.CreateDirectory(uploadDir);

                foreach (var file in productImages.Where(f => f.Length > 0))
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var path = Path.Combine(uploadDir, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    currentImages.Add("/images/products/" + fileName);
                }
            }

            // Gán lại ảnh sau khi xử lý
            existing.ImageUrls = string.Join(";", currentImages.Distinct());

            // XỬ LÝ ẢNH THUMBNAIL

            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(existing.ThumbNail))
                {
                    var oldThumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ThumbNail.TrimStart('/'));
                    if (System.IO.File.Exists(oldThumbPath)) System.IO.File.Delete(oldThumbPath);
                }

                var thumbDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/thumbnails");
                Directory.CreateDirectory(thumbDir);

                var newThumb = $"{Guid.NewGuid()}{Path.GetExtension(thumbnailFile.FileName)}";
                var pathThumb = Path.Combine(thumbDir, newThumb);
                using var stream = new FileStream(pathThumb, FileMode.Create);
                await thumbnailFile.CopyToAsync(stream);

                existing.ThumbNail = "/images/thumbnails/" + newThumb;
            }
            // Ngược lại giữ nguyên existing.ThumbNail

            // Lưu thay đổ
            _context.Update(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null) _context.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
            => _context.Products.Any(e => e.Id == id);
        /*Up load image*/
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(upload.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                var dirPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                using var stream = new FileStream(filePath, FileMode.Create);
                await upload.CopyToAsync(stream);

                var imageUrl = Url.Content("~/uploads/" + fileName);

                return Json(new
                {
                    uploaded = true,
                    url = imageUrl
                });
            }

            return Json(new
            {
                uploaded = false,
                error = new { message = "Upload failed" }
            });
        }
    }



}
