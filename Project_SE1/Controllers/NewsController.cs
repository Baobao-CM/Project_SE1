using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project_dnc_se1.Models;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace project_dnc_se1.Controllers
{
    public class NewsController : Controller
    {
        private readonly CompanyWebContext _context;
        private readonly IWebHostEnvironment _env;

        public NewsController(CompanyWebContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: News
        public IActionResult Index(string keyword, int? page, int? categoryId)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            ViewBag.Categories = _context.NewsCategories.ToList();
            ViewBag.SelectedCategoryId = categoryId;

            var news = _context.News
                                   .Include(p => p.Category)
                                   .Include(n => n.User)
                                   .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                news = news.Where(p => p.Title.Contains(keyword));

            if (categoryId.HasValue && categoryId > 0)
                news = news.Where(p => p.CategoryId == categoryId);

            var pagedList = news.OrderBy(p => p.Id)
                                    .ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }



        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Category)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.NewsCategories, "Id", "Title");
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news, IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var fileName = Path.GetFileName(upload.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "images/news", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                news.Banners = "/images/news/" + fileName;
            }
            news.UserId = HttpContext.Session.GetInt32("UserId");
            news.UpdatedAt = DateTime.Now;


            _context.Add(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.NewsCategories, "Id", "Title", news.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", news.UserId);
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Value,CategoryId,IsDeleted,Banners")] News news, IFormFile? BannersFile,
    string? OldBanners)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            //  Lấy UserId từ session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            news.UserId = userId.Value;
            news.UpdatedAt = DateTime.Now;

            // Nếu không chọn ảnh mới, giữ ảnh cũ
            if (BannersFile == null || BannersFile.Length == 0)
            {
                news.Banners = OldBanners;
            }

            ModelState.Remove("OldBanners"); // Loại bỏ lỗi validate không thuộc model

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu có ảnh mới, lưu lại
                    if (BannersFile != null && BannersFile.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(BannersFile.FileName)}";
                        var filePath = Path.Combine(_env.WebRootPath, "images/news", fileName);

                        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await BannersFile.CopyToAsync(stream);
                        }

                        news.Banners = "/images/news/" + fileName;
                    }

                    _context.Update(news);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.News.Any(e => e.Id == news.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.NewsCategories, "Id", "Title", news.CategoryId);
            return View(news);
        }


        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Category)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }

        /*Up load image*/
        [HttpPost]
        public async Task<IActionResult> UploadBanners(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(upload.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/news", fileName);

                var dirPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                using var stream = new FileStream(filePath, FileMode.Create);
                await upload.CopyToAsync(stream);

                var imageUrl = Url.Content("~/images/news/" + fileName);


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
