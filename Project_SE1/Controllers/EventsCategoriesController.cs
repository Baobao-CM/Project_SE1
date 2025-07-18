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
    public class EventsCategoriesController : Controller
    {
        private readonly CompanyWebContext _context;

        public EventsCategoriesController(CompanyWebContext context)
        {
            _context = context;
        }

        // GET: EventsCategories
        public async Task<IActionResult> Index()
        {

            var events = await _context.EventsCategories
                .Where(e => e.IsDeleted == false)
                .ToListAsync();

            return View(events);
        }

        // GET: EventsCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventsCategory = await _context.EventsCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventsCategory == null)
            {
                return NotFound();
            }

            return View(eventsCategory);
        }

        // GET: EventsCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EventsCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] EventsCategory eventsCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventsCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventsCategory);
        }

        // GET: EventsCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventsCategory = await _context.EventsCategories.FindAsync(id);
            if (eventsCategory == null)
            {
                return NotFound();
            }
            return View(eventsCategory);
        }

        // POST: EventsCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] EventsCategory eventsCategory)
        {
            if (id != eventsCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventsCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventsCategoryExists(eventsCategory.Id))
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
            return View(eventsCategory);
        }

        // GET: EventsCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventsCategory = await _context.EventsCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventsCategory == null)
            {
                return NotFound();
            }

            return View(eventsCategory);
        }

        // POST: EventsCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventsCategory = await _context.EventsCategories.FindAsync(id);
            if (eventsCategory != null)
            {
                _context.EventsCategories.Remove(eventsCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Trash));
        }

        private bool EventsCategoryExists(int id)
        {
            return _context.EventsCategories.Any(e => e.Id == id);
        }

        // POST: EventsCategories/SoftDelete/5
        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var ev = await _context.EventsCategories.FindAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            ev.IsDeleted = true;
            _context.Update(ev);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // POST: EventsCategories/Restore/5
        [HttpPost, ActionName("Restore")]
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var ev = await _context.EventsCategories.FindAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            ev.IsDeleted = false;
            _context.Update(ev);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Trash));
        }
        public async Task<IActionResult> Trash()
        {
            var deletedEvents = await _context.EventsCategories
                .Where(e => e.IsDeleted == true)
                .ToListAsync();

            return View(deletedEvents);
        }
    }
}
