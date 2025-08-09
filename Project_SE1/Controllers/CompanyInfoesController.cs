using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_dnc_se1.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace project_dnc_se1.Controllers
{
    public class CompanyInfoesController : Controller
    {
        private readonly CompanyWebContext _context;

        public CompanyInfoesController(CompanyWebContext context)
        {
            _context = context;
        }

        // GET: CompanyInfoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CompanyInfos.ToListAsync());
        }

        // GET: CompanyInfoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var companyInfo = await _context.CompanyInfos.FirstOrDefaultAsync(m => m.Id == id);
            if (companyInfo == null) return NotFound();

            return View(companyInfo);
        }

        // GET: CompanyInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CompanyInfoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Title, string valueType, string ValueText, List<IFormFile> ValueImage)
        {
            if (string.IsNullOrEmpty(Title))
            {
                ModelState.AddModelError("Title", "Tiêu đề không được để trống.");
                return View();
            }

            string value = string.Empty;

            if (valueType == "text")
            {
                value = ValueText;
            }
            else if (valueType == "image" && ValueImage != null && ValueImage.Count > 0)
            {
                List<string> imagePaths = new List<string>();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadPath);

                foreach (var image in ValueImage)
                {
                    var extension = Path.GetExtension(image.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension)) continue;

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    imagePaths.Add("/uploads/" + fileName);
                }

                value = string.Join(";", imagePaths); // nối đường dẫn ảnh bằng dấu ;
            }

            var companyInfo = new CompanyInfo
            {
                Title = Title,
                Value = value
            };

            _context.Add(companyInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: CompanyInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var companyInfo = await _context.CompanyInfos.FindAsync(id);
            if (companyInfo == null) return NotFound();

            ViewData["IsImage"] = companyInfo.Value?.StartsWith("/uploads/") == true;
            return View(companyInfo);
        }

        // POST: CompanyInfoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Title, string valueType, string ValueText, List<IFormFile> ValueImage)
        {
            var companyInfo = await _context.CompanyInfos.FindAsync(id);
            if (companyInfo == null) return NotFound();

            if (string.IsNullOrEmpty(Title))
            {
                ModelState.AddModelError("Title", "Tiêu đề không được để trống.");
                return View(companyInfo);
            }

            string value = companyInfo.Value;

            if (valueType == "text")
            {
                value = ValueText;
            }
            else if (valueType == "image" && ValueImage != null && ValueImage.Count > 0)
            {
                List<string> imagePaths = new List<string>();
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadPath);

                foreach (var image in ValueImage)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        imagePaths.Add("/uploads/" + fileName);
                    }
                }

                value = string.Join(";", imagePaths);
            }

            try
            {
                companyInfo.Title = Title;
                companyInfo.Value = value;
                _context.Update(companyInfo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyInfoExists(companyInfo.Id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: CompanyInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var companyInfo = await _context.CompanyInfos.FirstOrDefaultAsync(m => m.Id == id);
            if (companyInfo == null) return NotFound();

            return View(companyInfo);
        }

        // POST: CompanyInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companyInfo = await _context.CompanyInfos.FindAsync(id);
            if (companyInfo != null)
            {
                _context.CompanyInfos.Remove(companyInfo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CompanyInfoExists(int id)
        {
            return _context.CompanyInfos.Any(e => e.Id == id);
        }
    }
}
