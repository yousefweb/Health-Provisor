using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using HealthProvisor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MimeKit;
using System.Numerics;
using HealthProvisor.Data.Enum;

namespace HealthProvisor.Controllers
{
    public class ConsultationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ConsultationsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Consultations
        public async Task<IActionResult> Index()
        {
          
            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user is a doctor
            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DOCTOR"))
            {
                // Get the doctor ID of the current user
                var doctorId = _context.Doctors
                .Where(d => d.UserId == currentUser.Id)
                .Select(d => d.DoctorID)
                .FirstOrDefault();

                if (doctorId != null)
                {
                   
                    ViewBag.Consultations = await _context.Consultations
                        .Include(c => c.Doctor)
                            .ThenInclude(d => d.User)
                        .Include(c => c.Patient)
                            .ThenInclude(p => p.User)
                        .Include(p => p.Visa)
                        .Where(c => c.DoctorID == doctorId && c.Patient_Status == "Accept" && c.IsSubmitted==true)
                        .ToListAsync();
                }
                  
                return View();

            }
              
                return RedirectToAction("Error", "Home");
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
                .ThenInclude(c=>c.User)
                .Include(c => c.Patient)
                .ThenInclude(c=>c.User)
                .Include(c=>c.Visa)
                .FirstOrDefaultAsync(m => m.ConsultationID == id);
            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }

        public IActionResult Create(int id)
        {
          ViewBag.CategoryId = id;

            var doctors = _context.Doctors
           .Include(u => u.User)
           .Where(d => d.CategoryId == id && d.DoctorStatus =="Accept")
           .ToList();

            ViewBag.Doctors = doctors;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Consultation consultation, int id)
        {
           

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var patient = _context.Patients.FirstOrDefault(p => p.User.Email == user.Email);

                if (patient != null)
                {

                    ViewBag.Doctors = _context.Doctors
                    .Include(u => u.User)
                    .Where(d => d.CategoryId == id && d.DoctorStatus == "Accept")
                    .ToList();

                    if (!ModelState.IsValid)
                    {
                        foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                        {
                            Console.WriteLine($"Error: {modelError.ErrorMessage}");
                        }
                    }

                    else
                    {
                        var visa = await _context.Visas.Include(v => v.Patient).ThenInclude(p => p.User).Where(v => v.PatientID == patient.PatientID).FirstOrDefaultAsync();

                        if (visa != null) {

                            _context.Consultations.Add(new Consultation
                            {
                                ConsultationID = consultation.ConsultationID,
                                DoctorID = consultation.DoctorID,
                                PatientID = patient.PatientID,
                                VisaId = visa.VisaId,
                                FirstName = consultation.FirstName,
                                LastName = consultation.LastName,
                                Patient_Gender = consultation.Patient_Gender,
                                Patient_Age = consultation.Patient_Age,
                                Patient_Status = "Accept",
                                IsSubmitted = false,
                                Notes = consultation.Notes,
                                Date = consultation.Date

                            }) ;

                        await _context.SaveChangesAsync();


                        return RedirectToAction("Proceed", "Visas");
                    }
                }
                }
            }

            return View(consultation);
        }

        [HttpPost]
        public IActionResult GetDoctorSpecialization(int doctorId)
        {
            var specializationValue = _context.Doctors
                .Where(d => d.DoctorID == doctorId)
                .Select(d => d.Doctor_Specialization)
                .FirstOrDefault();

            if (Enum.IsDefined(typeof(MedicalSpecialization),specializationValue))
            {
                var specialization = Enum.GetName(typeof(MedicalSpecialization), specializationValue);
                return Json(specialization);
            }

            return Json(null); 
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
                .ThenInclude(c =>c.User)
                .Include(c => c.Patient)
                .ThenInclude(c =>c.User)
                .Include(c=>c.Visa)
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
