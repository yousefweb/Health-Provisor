using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using HealthProvisor.Models;

namespace HealthProvisor.Controllers
{
    public class ConsultationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Consultations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Consultations.Include(c => c.Doctor).Include(c => c.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Consultations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Consultations == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations
                .Include(c => c.Doctor)
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(m => m.ConsultationID == id);
            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }

        // GET: Consultations/Create
        public IActionResult Create()
        {
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "DoctorStatus");
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID");
            return View();
        }

        // POST: Consultations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsultationID,PatientID,DoctorID,Date,Notes")] Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consultation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "DoctorStatus", consultation.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", consultation.PatientID);
            return View(consultation);
        }

        // GET: Consultations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Consultations == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound();
            }
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "DoctorStatus", consultation.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", consultation.PatientID);
            return View(consultation);
        }

        // POST: Consultations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsultationID,PatientID,DoctorID,Date,Notes")] Consultation consultation)
        {
            if (id != consultation.ConsultationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consultation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultationExists(consultation.ConsultationID))
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
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "DoctorStatus", consultation.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", consultation.PatientID);
            return View(consultation);
        }

        // GET: Consultations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Consultations == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations
                .Include(c => c.Doctor)
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(m => m.ConsultationID == id);
            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }

        // POST: Consultations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Consultations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Consultations'  is null.");
            }
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation != null)
            {
                _context.Consultations.Remove(consultation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsultationExists(int id)
        {
          return (_context.Consultations?.Any(e => e.ConsultationID == id)).GetValueOrDefault();
        }
    }
}
