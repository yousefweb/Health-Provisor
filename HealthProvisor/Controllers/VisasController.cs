using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using MimeKit;
using Microsoft.AspNetCore.Identity;
using HealthProvisor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HealthProvisor.Controllers
{
   
    public class VisasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public VisasController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public IActionResult Create()
        {
           
                var user = _userManager.GetUserAsync(User).Result;
                if (user != null)
                {
                    var patient = _context.Patients.FirstOrDefault(x => x.User.Email == user.Email);
                 //   var consultation = _context.Consultations.Include(c => c.Patient).ThenInclude(c => c.User).Include(c => c.Doctor).ThenInclude(c => c.User).Where(c => c.PatientID == patient.PatientID).OrderByDescending(c=>c.Date).FirstOrDefault();

                    if (patient != null)
                    {
                        var patientInfo = new Tuple<int, string>(patient.PatientID, $"{user.FirstName} {user.LastName}");
                        ViewBag.PatientInfo = patientInfo;
                    }
                    else
                    {
                        ViewBag.PatientInfo = null;
                    }
                }
                else
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
            
            return View(new Visa());
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visa visa,int id)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var existingVisa = await _context.Visas.FirstOrDefaultAsync(v => v.PatientID == visa.PatientID);

                if (existingVisa != null)
                {
                    if (existingVisa.Equalss(visa))
                    {
                        return RedirectToAction("Create", "Consultations", new { id = id });

                        //return RedirectToAction(nameof(Proceed));
                        //return Json(new { success = true, message = "Visa info already exists and matches the provided one." });
                    }
                    else
                    {
                        try
                        {
                            
                            existingVisa.VisaNumber = visa.VisaNumber;
                            existingVisa.CVC = visa.CVC;
                            existingVisa.ExpDate = visa.ExpDate;
                            existingVisa.TransactionDate = visa.TransactionDate;

                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred while updating the visa entity: " + ex.Message);
                            return View(visa);
                            //return Json(new { success = false, error = "An error occurred while updating the visa entity. Please try again." });
                        }
                    }
                }
                else
                {
                   
                    await _context.AddAsync(visa);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Create", "Consultations", new { id = id });
                //return RedirectToAction(nameof(Proceed));
                //return Json(new { success = true, message = "Visa info saved successfully." });

            }


            foreach (var entry in ModelState)
            {
                if (entry.Value.Errors.Count > 0)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        ModelState.AddModelError(entry.Key, error.ErrorMessage);
                    }
                }
            }

            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "UserId", visa.PatientID);
            return View(visa);
            //return Json(new { success = false, error = "error occurred check data field is valid and not empty." });
        }
        [HttpGet]
        public async Task<IActionResult> Proceed()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var visa1 = await _context.Visas.Include(v => v.Patient).ThenInclude(v => v.User).FirstOrDefaultAsync(v => v.Patient.UserId == user.Id);
            if (visa1 == null)
            {
                return NotFound();
            }
            ViewBag.FirstName = visa1.Patient.User?.FirstName;
            ViewBag.LastName = visa1.Patient.User?.LastName;
            ViewBag.FullName = $"{visa1.Patient.User?.FirstName} {visa1.Patient.User?.LastName}";
            ViewBag.Email = visa1.Patient.User?.Email;
            ViewBag.PhoneNumber = visa1.Patient.User?.PhoneNumber;
            ViewBag.CVC = visa1.CVC;
            ViewBag.VisaNumber = visa1.VisaNumber;
            ViewBag.ExpDate = visa1.ExpDate;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Proceed(Visa visa)
        {
            if (visa == null)
            {
                return BadRequest("Visa data is missing.");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var visa1 = await _context.Visas.Include(v => v.Patient).ThenInclude(v => v.User).FirstOrDefaultAsync(v => v.Patient.UserId == user.Id);
            if (visa1 == null)
            {
                return NotFound();
            }


            var consultation = await _context.Consultations
                .FirstOrDefaultAsync(c => c.Patient.UserId == user.Id);

            if (consultation == null)
            {
                return NotFound();
            }
            consultation.IsSubmitted = true;
            await _context.SaveChangesAsync();
            ViewBag.FirstName = visa1.Patient.User?.FirstName;
            ViewBag.LastName = visa1.Patient.User?.LastName;
            ViewBag.FullName = $"{visa1.Patient.User?.FirstName} {visa1.Patient.User?.LastName}";
            ViewBag.Email = visa1.Patient.User?.Email;
            ViewBag.PhoneNumber = visa1.Patient.User?.PhoneNumber;
            ViewBag.CVC = visa1.CVC;
            ViewBag.VisaNumber = visa1.VisaNumber;
            ViewBag.ExpDate = visa1.ExpDate;
            await SendPatientStatusConfirmationEmail(visa1);
            return Json(new { success = true });
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
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "UserId", visa.PatientID);
            return View(visa);
        }

        // POST: Visas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VisaId,CVC,VisaNumber,ExpDate,TransactionDate,PatientID")] Visa visa)
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
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "UserId", visa.PatientID);
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
                .ThenInclude(v=>v.User)
                .FirstOrDefaultAsync(m => m.VisaId == id);
          

            if (visa == null)
            {
                return NotFound();
            }
            ViewBag.VisaNumber = visa.VisaNumber;
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
            return RedirectToAction("Profile","Patients");
        }

        private bool VisaExists(int id)
        {
            return (_context.Visas?.Any(e => e.VisaId == id)).GetValueOrDefault();
        }

        public async Task SendPatientStatusConfirmationEmail(Visa visa)
        {

            var pat = await _context.Patients.Include(d => d.User).FirstOrDefaultAsync(x => x.PatientID == visa.PatientID);
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("HealthProvisor", "HealthProvisor@gmail.com"));
            email.To.Add(new MailboxAddress($"{pat.User.FirstName} {pat.User.LastName}", pat.User.Email));
            email.Subject = "Status Confirmation";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Dear {pat.User.FirstName} {pat.User.LastName},\n\n"
                     + "Thank you for Consulting with us!\n\n"
                     + $"Your Request has been Confirmed.\n"
                     + $"And the Payment Process has been Completed successfully.\n"
                     + $"During 24 hours you will receive the consultation from your doctor.\n"
                     + "If you have any questions or need assistance or faced any problem, please feel free to contact us at HealthProvisor@gmail.com.\n\n"
                     + "We appreciate your taking the time for Consulting with us and look forward to serving you again!\n\n"
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
