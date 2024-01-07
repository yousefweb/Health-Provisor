using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using _5Dots.Models;

namespace HealthProvisor.Controllers
{
    public class VisasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Visas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Visas.Include(v => v.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Visas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Visas == null)
            {
                return NotFound();
            }

            var visa = await _context.Visas
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(m => m.VisaId == id);
            if (visa == null)
            {
                return NotFound();
            }

            return View(visa);
        }

        // GET: Visas/Create
        public IActionResult Create()
        {
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID");
            return View();
        }

        // POST: Visas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VisaId,CVC,VisaNumber,ExpDate,Amount,TransactionDate,PatientID")] Visa visa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", visa.PatientID);
            return View(visa);
        }

        // GET: Visas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Visas == null)
            {
                return NotFound();
            }

            var visa = await _context.Visas.FindAsync(id);
            if (visa == null)
            {
                return NotFound();
            }
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", visa.PatientID);
            return View(visa);
        }

        // POST: Visas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VisaId,CVC,VisaNumber,ExpDate,Amount,TransactionDate,PatientID")] Visa visa)
        {
            if (id != visa.VisaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisaExists(visa.VisaId))
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
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", visa.PatientID);
            return View(visa);
        }

        // GET: Visas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Visas == null)
            {
                return NotFound();
            }

            var visa = await _context.Visas
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(m => m.VisaId == id);
            if (visa == null)
            {
                return NotFound();
            }

            return View(visa);
        }

        // POST: Visas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Visas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Visas'  is null.");
            }
            var visa = await _context.Visas.FindAsync(id);
            if (visa != null)
            {
                _context.Visas.Remove(visa);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisaExists(int id)
        {
          return (_context.Visas?.Any(e => e.VisaId == id)).GetValueOrDefault();
        }
    }
}
