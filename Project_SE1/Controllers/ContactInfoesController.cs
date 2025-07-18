using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project_dnc_se1.Models;

namespace project_dnc_se1.Controllers
{
    public class ContactInfoesController : Controller
    {
        private readonly CompanyWebContext _context;

        public ContactInfoesController(CompanyWebContext context)
        {
            _context = context;
        }

        // GET: ContactInfoes
        public async Task<IActionResult> Index()
        {
            var companyWebContext = _context.ContactInfos.Include(c => c.Events).Include(c => c.Product);
            return View(await companyWebContext.ToListAsync());
        }

        // GET: ContactInfoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactInfo = await _context.ContactInfos
                .Include(c => c.Events)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactInfo == null)
            {
                return NotFound();
            }

            return View(contactInfo);
        }

        // GET: ContactInfoes/Create
        public IActionResult Create()
        {
            ViewData["EventsId"] = new SelectList(_context.Events, "Id", "Title");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Title");
            return View();
        }

        // POST: ContactInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Email,Message,ProductId,EventsId")] ContactInfo contactInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventsId"] = new SelectList(_context.Events, "Id", "Title", contactInfo.EventsId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Title", contactInfo.ProductId);
            return View(contactInfo);
        }

        // GET: ContactInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactInfo = await _context.ContactInfos.FindAsync(id);
            if (contactInfo == null)
            {
                return NotFound();
            }
            ViewData["EventsId"] = new SelectList(_context.Events, "Id", "Title", contactInfo.EventsId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Title", contactInfo.ProductId);
            return View(contactInfo);
        }

        // POST: ContactInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Email,Message,ProductId,EventsId")] ContactInfo contactInfo)
        {
            if (id != contactInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactInfoExists(contactInfo.Id))
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
            ViewData["EventsId"] = new SelectList(_context.Events, "Id", "Title", contactInfo.EventsId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Title", contactInfo.ProductId);
            return View(contactInfo);
        }

        // GET: ContactInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactInfo = await _context.ContactInfos
                .Include(c => c.Events)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactInfo == null)
            {
                return NotFound();
            }

            return View(contactInfo);
        }

        // POST: ContactInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactInfo = await _context.ContactInfos.FindAsync(id);
            if (contactInfo != null)
            {
                _context.ContactInfos.Remove(contactInfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactInfoExists(int id)
        {
            return _context.ContactInfos.Any(e => e.Id == id);
        }
    }
}
