using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using project_dnc_se1.Models;
using X.PagedList.Extensions;

namespace project_dnc_se1.Controllers
{
    public class EventsController : Controller
    {
        private readonly CompanyWebContext _context;
        private readonly IWebHostEnvironment _env;
        public EventsController(CompanyWebContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Events
        public IActionResult Index(string keyword, int? page, int? categoryId)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            ViewBag.Categories = _context.EventsCategories.ToList();
            ViewBag.SelectedCategoryId = categoryId;

            var events = _context.Events
                                   .Where(p => p.IsDeleted == false)
                                   .Include(p => p.Category)
                                   .Include(n => n.User)
                                   .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                events = events.Where(p => p.Title.Contains(keyword));

            if (categoryId.HasValue && categoryId > 0)
                events = events.Where(p => p.CategoryId == categoryId);

            var pagedList = events.OrderBy(p => p.Id)
                                    .ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }


        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.EventsCategories, "Id", "Title");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event events, IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var fileName = Path.GetFileName(upload.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "images/events", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                events.Banners = "/images/events/" + fileName;
            }
            events.UserId = HttpContext.Session.GetInt32("UserId");
            events.UpdatedAt = DateTime.Now;

            _context.Add(events);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.EventsCategories, "Id", "Title", @event.CategoryId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event, IFormFile? BannersFile,
    string? OldBanners)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }
            var existing = await _context.Events.FindAsync(id);
            if (existing == null) return NotFound();
            //  Gán người dùng từ session (nếu chưa đăng nhập thì chuyển hướng)
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }
            @event.UserId = userId.Value;
            @event.UpdatedAt = DateTime.Now;

            // Nếu không chọn ảnh mới thì gán ảnh cũ
            if (string.IsNullOrEmpty(@event.Banners) && string.IsNullOrEmpty(OldBanners))
            {
                // Không có ảnh mới và không có ảnh cũ => có thể cho phép hoặc báo lỗi
                ModelState.AddModelError("Banners", "Bạn chưa chọn ảnh hoặc chưa có ảnh cũ.");
            }

            // Loại bỏ lỗi validate không cần thiết (nếu có)
            ModelState.Remove("OldBanners");

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.EventsCategories, "Id", "Title", @event.CategoryId);
                return View(@event);

            }
            // Cập nhật các trường cơ bản
            existing.Title = @event.Title;
            existing.Value = @event.Value;
            existing.CategoryId = @event.CategoryId;
            existing.UpdatedAt = DateTime.Now;
            try
            {
                //  Nếu có file mới → xử lý lưu ảnh
                if (BannersFile != null && BannersFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(BannersFile.FileName)}";
                    var filePath = Path.Combine(_env.WebRootPath, "images/events", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await BannersFile.CopyToAsync(stream);
                    }

                    existing.Banners = "/images/events/" + fileName;
                }
                else
                {
                    // Không có ảnh mới → giữ ảnh cũ
                    existing.Banners = OldBanners;
                }

                _context.Update(existing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(existing.Id))
                    return NotFound();
                else
                    throw;
            }

        }


        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        /*Up load image*/
        [HttpPost]
        public async Task<IActionResult> UploadBanners(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(upload.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/events", fileName);

                var dirPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                using var stream = new FileStream(filePath, FileMode.Create);
                await upload.CopyToAsync(stream);

                var banners = Url.Content("~/images/events/" + fileName);

                return Json(new
                {
                    uploaded = true,
                    url = banners
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
