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
    public class EventsProductsController : Controller
    {
        private readonly CompanyWebContext _context;

        public EventsProductsController(CompanyWebContext context)
        {
            _context = context;
        }

        // GET: EventsProducts
        public async Task<IActionResult> Index()
        {
            var companyWebContext = _context.EventsProducts.Include(e => e.Events).Include(e => e.Product);
            return View(await companyWebContext.ToListAsync());
        }

        // GET: EventsProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventsProduct = await _context.EventsProducts
                .Include(e => e.Events)
                .Include(e => e.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventsProduct == null)
            {
                return NotFound();
            }

            return View(eventsProduct);
        }

        // GET: EventsProducts/Create
        public IActionResult Create()
        {
            ViewData["EventsId"] = new SelectList(_context.Events, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: EventsProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventsId,ProductId")] EventsProduct eventsProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventsProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventsId"] = new SelectList(_context.Events, "Id", "Id", eventsProduct.EventsId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", eventsProduct.ProductId);
            return View(eventsProduct);
        }

        // GET: EventsProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventsProduct = await _context.EventsProducts.FindAsync(id);
            if (eventsProduct == null)
            {
                return NotFound();
            }
            ViewData["EventsId"] = new SelectList(_context.Events, "Id", "Id", eventsProduct.EventsId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", eventsProduct.ProductId);
            return View(eventsProduct);
        }

        // POST: EventsProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventsId,ProductId")] EventsProduct eventsProduct)
        {
            if (id != eventsProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventsProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventsProductExists(eventsProduct.Id))
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
            ViewData["EventsId"] = new SelectList(_context.Events, "Id", "Id", eventsProduct.EventsId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", eventsProduct.ProductId);
            return View(eventsProduct);
        }

        // GET: EventsProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventsProduct = await _context.EventsProducts
                .Include(e => e.Events)
                .Include(e => e.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventsProduct == null)
            {
                return NotFound();
            }

            return View(eventsProduct);
        }

        // POST: EventsProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventsProduct = await _context.EventsProducts.FindAsync(id);
            if (eventsProduct != null)
            {
                _context.EventsProducts.Remove(eventsProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventsProductExists(int id)
        {
            return _context.EventsProducts.Any(e => e.Id == id);
        }
    }
}
