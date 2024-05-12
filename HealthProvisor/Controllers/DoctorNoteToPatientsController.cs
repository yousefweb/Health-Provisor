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
using MimeKit;

namespace HealthProvisor.Controllers
{
    public class DoctorNoteToPatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DoctorNoteToPatientsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DoctorNoteToPatients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DoctorNoteToPatients.Include(d => d.Consultation).Include(d => d.Doctor).Include(d => d.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DoctorNoteToPatients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DoctorNoteToPatients == null)
            {
                return NotFound();
            }

            var doctorNoteToPatient = await _context.DoctorNoteToPatients
                .Include(d => d.Consultation)
                .Include(d => d.Doctor)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctorNoteToPatient == null)
            {
                return NotFound();
            }

            return View(doctorNoteToPatient);
        }

        // GET: DoctorNoteToPatients/Create
        public IActionResult Create()
        {
            ViewData["ConsultationID"] = new SelectList(_context.Consultations, "ConsultationID", "FirstName");
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "DoctorID");
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID");
            return View();
        }

        // POST: DoctorNoteToPatients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ConsultationDetails,DoctorNote,MessageTime,PatientID,ConsultationID,DoctorID")] DoctorNoteToPatient doctorNoteToPatient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctorNoteToPatient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsultationID"] = new SelectList(_context.Consultations, "ConsultationID", "FirstName", doctorNoteToPatient.ConsultationID);
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "DoctorID", doctorNoteToPatient.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", doctorNoteToPatient.PatientID);
            return View(doctorNoteToPatient);
        }

        // GET: DoctorNoteToPatients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DoctorNoteToPatients == null)
            {
                return NotFound();
            }

            var doctorNoteToPatient = await _context.DoctorNoteToPatients.FindAsync(id);
            if (doctorNoteToPatient == null)
            {
                return NotFound();
            }
            ViewData["ConsultationID"] = new SelectList(_context.Consultations, "ConsultationID", "FirstName", doctorNoteToPatient.ConsultationID);
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "DoctorID", doctorNoteToPatient.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", doctorNoteToPatient.PatientID);
            return View(doctorNoteToPatient);
        }

        // POST: DoctorNoteToPatients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConsultationDetails,DoctorNote,MessageTime,PatientID,ConsultationID,DoctorID")] DoctorNoteToPatient doctorNoteToPatient)
        {
            if (id != doctorNoteToPatient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorNoteToPatient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorNoteToPatientExists(doctorNoteToPatient.Id))
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
            ViewData["ConsultationID"] = new SelectList(_context.Consultations, "ConsultationID", "FirstName", doctorNoteToPatient.ConsultationID);
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "DoctorID", doctorNoteToPatient.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", doctorNoteToPatient.PatientID);
            return View(doctorNoteToPatient);
        }

        // GET: DoctorNoteToPatients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DoctorNoteToPatients == null)
            {
                return NotFound();
            }

            var doctorNoteToPatient = await _context.DoctorNoteToPatients
                .Include(d => d.Consultation)
                .Include(d => d.Doctor)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctorNoteToPatient == null)
            {
                return NotFound();
            }

            return View(doctorNoteToPatient);
        }

        // POST: DoctorNoteToPatients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DoctorNoteToPatients == null)
            {
                return Problem("Entity set 'ApplicationDbContext.DoctorNoteToPatients'  is null.");
            }
            var doctorNoteToPatient = await _context.DoctorNoteToPatients.FindAsync(id);
            if (doctorNoteToPatient != null)
            {
                _context.DoctorNoteToPatients.Remove(doctorNoteToPatient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorNoteToPatientExists(int id)
        {
            return (_context.DoctorNoteToPatients?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> DoctorNote()
        {

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DOCTOR"))
            {
                var doctorId = _context.Doctors
                    .Where(d => d.UserId == currentUser.Id)
                    .Select(d => d.DoctorID)
                    .FirstOrDefault();

                if (doctorId != null)
                {
                    ViewBag.Consult = _context.Consultations
                    .Include(n => n.Doctor)
                    .ThenInclude(n => n.User)
                    .Include(n => n.Patient)
                    .ThenInclude(n => n.User)
                    .Include(c=>c.Visa)
                    .Where(p =>  p.DoctorID == doctorId && p.Patient_Status == "Accept" && p.IsSubmitted == true)
                    .Distinct().ToList();

                    return View();
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoctorNote(DoctorNoteToPatient doctorNoteToPatient, int patientId)
        {
       
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "DOCTOR"))
            {
                return Unauthorized(); 
            }

            var doctorId = _context.Doctors
                .Where(d => d.UserId == currentUser.Id)
                .Select(d => d.DoctorID)
                .FirstOrDefault();

            var patient = _context.Patients
          .Include(p => p.Consultations)
          .FirstOrDefault(p => p.PatientID == patientId);


            if (patient == null)
            {
                return NotFound("No patient with consultation found"); 
            }
            var consultation = patient.Consultations.FirstOrDefault(c => c.Patient_Status == "Accept");
            if (consultation == null)
            {
                return NotFound("No consultation found with 'Accept' status for the patient.");
            }

            var newDoctorNote = new DoctorNoteToPatient
            {
                PatientID = patient.PatientID,
                DoctorID = doctorId,
                ConsultationID = consultation.ConsultationID,
                ConsultationDetails = doctorNoteToPatient.ConsultationDetails,
                DoctorNote = doctorNoteToPatient.DoctorNote,
                MessageTime = doctorNoteToPatient.MessageTime 
            };

            _context.DoctorNoteToPatients.Add(newDoctorNote);

            try
            {
           
                await _context.SaveChangesAsync();
                await SendPatientStatusConfirmationEmail(newDoctorNote);
                return Json(new { success = true }); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving changes: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

              
                return Json(new { success = false, error = "An error occurred while saving changes. Please try again later." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetPatientDetails(int patientId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DOCTOR"))
            {
                var doctorId = _context.Doctors
                    .Where(d => d.UserId == currentUser.Id)
                    .Select(d => d.DoctorID)
                    .FirstOrDefault();

                var Consult = await _context.Consultations
                    .Include(c => c.Patient)
                        .ThenInclude(p => p.User)
                    .Include(c => c.Doctor)
                        .ThenInclude(d => d.Category)
                    .Include(c => c.Doctor)
                        .ThenInclude(d => d.User)
                        .Include(c=>c.Visa)
                    .Where(c => c.PatientID == patientId && c.DoctorID == doctorId && c.Patient_Status == "Accept" && c.IsSubmitted == true)
                    .ToListAsync();

           
                if (Consult != null)
                {
                    var patientDetails = "";
                    foreach (var consult in Consult)
                    {
                        patientDetails += $"Consultation sent to Dr. {consult.Doctor.User?.FirstName} {consult.Doctor.User?.LastName}:\n\n";
                        patientDetails += $"Patient Name: {consult.FirstName} {consult.LastName}, \n\n";
                        patientDetails += $"Date: {consult.Date}, \n\n";
                        patientDetails += $"Details: {consult.Notes} \n\n";
                        patientDetails += ".........................................\n\n";
                    }

                    return Json(patientDetails);
                }
                return Json(null);
            }

            return Json(null);
        }

        public async Task SendPatientStatusConfirmationEmail(DoctorNoteToPatient doctorNoteToPatient)
        {
            var user = await _userManager.GetUserAsync(User);
            var note = await _context.DoctorNoteToPatients
                .Include(n => n.Consultation)
                    .ThenInclude(n => n.Patient)
                        .ThenInclude(u => u.User)
                .Include(n => n.Doctor)
                    .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(x => x.PatientID == doctorNoteToPatient.PatientID && x.Doctor.User.Id == user.Id);


            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("HealthProvisor", "HealthProvisor@gmail.com"));
            email.To.Add(new MailboxAddress($"{note.Patient.User.FirstName} {note.Patient.User.LastName}", note.Patient.User.Email));
            email.Subject = "Status Confirmation";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Dear {note.Patient.User.FirstName} {note.Patient.User.LastName},\n\n"
                                 + "Thank you for Consulting with us!\n\n"
                                 + $"Your Status has been Confirmed.\n"
                                 + $"The Doctor {note.Doctor.User.FirstName} {note.Doctor.User.LastName} has responded to your message. Please check your profile for details.\n\n"
                                 + "Best regards,\n"
                                 + "Health Provisor Team";

            email.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate("HealthProvisor@gmail.com", "");
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
        }

    }
}
