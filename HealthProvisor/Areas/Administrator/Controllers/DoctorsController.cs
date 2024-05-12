using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using System.Security.Claims;
using HealthProvisor.Models;
using MimeKit;
using System.Net.Mail;
using System.Security.Claims;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
namespace HealthProvisor.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Administrator/Doctors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Doctors.Include(d => d.Category).Include(d => d.User).Where(d=>d.DoctorStatus =="Accept");
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Administrator/Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Category)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DoctorID == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Administrator/Doctors/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Administrator/Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorID,UserId,CategoryId,Doctor_Specialization,Doctor_Age,Doctor_Gender,Doctor_YearsOfExperience,DoctorStatus,ImageName,ContentType,Image")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", doctor.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", doctor.UserId);
            return View(doctor);
        }

        // GET: Administrator/Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", doctor.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", doctor.UserId);
            return View(doctor);
        }

        // POST: Administrator/Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorID,UserId,CategoryId,Doctor_Specialization,Doctor_Age,Doctor_Gender,Doctor_YearsOfExperience,DoctorStatus,ImageName,ContentType,Image")] Doctor doctor)
        {
            if (id != doctor.DoctorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorID))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", doctor.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", doctor.UserId);
            return View(doctor);
        }

        // GET: Administrator/Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Category)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DoctorID == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Administrator/Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Doctors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Doctors' is null.");
            }

            var doctor = await _context.Doctors
                .Include(d => d.Category)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DoctorID == id);

            if (doctor != null)
            {
                var userId = doctor.User?.Id;

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _context.Users.FindAsync(userId);

                    if (user != null)
                    {
                        var userRoles = await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
                        _context.UserRoles.RemoveRange(userRoles);

                        _context.Users.Remove(user);
                    }
                }

                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }

        private bool DoctorExists(int id)
        {
            return (_context.Doctors?.Any(e => e.DoctorID == id)).GetValueOrDefault();
        }


        public IActionResult DoctorRequest()
        {
            ViewBag.Doctors = _context.Doctors.Include(doctor => doctor.User).ToList();
            return View(ViewBag.Doctors);
        }


        public async Task<IActionResult> EditDoctorStatus(int DoctorID, string newStatus)
        {
            var doctor = await _context.Doctors.FindAsync(DoctorID);

            if (doctor == null)
            {
                return NotFound();
            }

            string currentStatus = doctor.DoctorStatus;

            if (newStatus == "Accept")
            {

                doctor.DoctorStatus = "Accept";
                _context.Update(doctor);
                await _context.SaveChangesAsync();

            }
            else if (newStatus == "Reject")
            {

                doctor.DoctorStatus = "Reject";
                _context.Update(doctor);
                await _context.SaveChangesAsync();

            }
            else if (newStatus == "Pending")
            {
                doctor.DoctorStatus = "Pending";
                _context.Update(doctor);
                await _context.SaveChangesAsync();
            }
            if (newStatus != currentStatus)
            {
                doctor.DoctorStatus = newStatus;
                _context.Update(doctor);
                await _context.SaveChangesAsync();

                await SendDoctorStatusConfirmationEmail(doctor);
            }
            return RedirectToAction(nameof(DoctorRequest));
        }
        public async Task SendDoctorStatusConfirmationEmail(Doctor doctor)
        {

            Doctor doc = _context.Doctors.Include(d => d.User).FirstOrDefault(x => x.DoctorID == doctor.DoctorID);
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("HealthProvisor", "HealthProvisor@gmail.com"));
            email.To.Add(new MailboxAddress($"{doc.User.FirstName} {doc.User.LastName}", doc.User.Email));
            email.Subject = "Status Confirmation";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Dear {doc.User.FirstName} {doc.User.LastName},\n\n"
                             + "Thank you for choosing us!\n\n"
                             + $"Your Status is {doctor.DoctorStatus} has been Confirmed.\n"
                             + "We appreciate your taking a time for work with us and look forward to serving you again!\n\n"
                             + "Good luck to you and your journey with us\n"
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
