using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using Microsoft.AspNetCore.Identity;
using HealthProvisor.Data.Enum;
using HealthProvisor.Models;
using HealthProvisor.Models.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MimeKit;
using System.Numerics;
using SQLitePCL;
namespace HealthProvisor.Controllers
{
    [Authorize(Roles = "PATIENT")]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PatientsController> _logger;
        private readonly SignInManager<User> _signInManager;
        public PatientsController(ApplicationDbContext context, UserManager<User> userManager, ILogger<PatientsController> logger, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        // GET: Patients
        public async Task<IActionResult> Index()
        {
            return _context.Patients != null ?
                        View(await _context.Patients.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Patients'  is null.");
        }
        [AllowAnonymous]
        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.Include(c => c.User)
                .FirstOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
        [AllowAnonymous]
        // GET: Patients/Create
        public IActionResult Create()
        {
            ViewBag.gender = Enum.GetValues(typeof(Gender));
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientRegisterViewModel model)
        {

            if (ModelState.IsValid)
            {

                if (_context.Users.Any(u => u.UserName == model.UserName))
                {
                    ModelState.AddModelError("UserName", "UserName is already taken");
                    return View(model);
                }

                var user = new User
                {

                    Email = model.Email,
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true,
                    NormalizedEmail = model.Email?.ToUpper(),
                    NormalizedUserName = model.UserName?.ToUpper(),
                    Role = "PATIENT"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var patient = new Patient
                    {
                        User = user,
                        Patient_Age = model.Age,
                        Patient_Gender = model.Gender,
                        Patient_Status = "Accept"
                    };

                    using (var stream = model.FormFile.OpenReadStream())
                    using (var reader = new BinaryReader(stream))
                    {
                        var byteFile = reader.ReadBytes((int)stream.Length);
                        patient.Image = byteFile;
                    }

                    patient.ImageName = model.FormFile.FileName;
                    patient.ContentType = model.FormFile.ContentType;

                    _context.Patients.Add(patient);

                    var roleId = "3";
                    var userRole = new IdentityUserRole<string>
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    };

                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User and patient created successfully.");
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error during user creation: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            return View(model);
        }

        [AllowAnonymous]
        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientID,Patient_Age,Patient_Gender,ImageName,ContentType,Image")] Patient patient)
        {
            if (id != patient.PatientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientID))
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
            return View(patient);
        }
        [AllowAnonymous]
        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.Include(x => x.User)
                .FirstOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [AllowAnonymous]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Patients == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Patients'  is null.");
            }
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        private bool PatientExists(int id)
        {
            return (_context.Patients?.Any(e => e.PatientID == id)).GetValueOrDefault();
        }
        [AllowAnonymous]
        public async Task<IActionResult> PatientRequest()
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
                    ViewBag.Consultations = _context.Consultations
                        .Include(c => c.Patient).ThenInclude(c => c.User)
                        .Include(c => c.Doctor).ThenInclude(c => c.User)
                        .Include(c =>c.Visa)
                        .Where(c => c.DoctorID == doctorId && c.IsSubmitted == true).ToList();


                    return View();
                }
            }
            return RedirectToAction("Error", "Home");
        }
        [AllowAnonymous]
        public async Task<IActionResult> EditPatientStatus(int ConsultationID, string newStatus)
        {

            var consult = await _context.Consultations.FindAsync(ConsultationID);

            if (consult == null)
            {
                return NotFound();
            }

            if (newStatus == "Accept")
            {

                consult.Patient_Status = "Accept";
                _context.Update(consult);
                await _context.SaveChangesAsync();
            }
            else
            {
                consult.Patient_Status = "Completed";
                _context.Update(consult);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(PatientRequest));
        }



        #region Profile
        [HttpGet]
        public IActionResult Profile()
        {

            var user = _userManager.GetUserAsync(User).Result;

            if (user != null)
            {
                ViewBag.NoteResponse = _context.DoctorNoteToPatients.Include(n => n.Doctor)
                                        .ThenInclude(n => n.User)
                                        .Include(n => n.Consultation).Include(n => n.Patient)
                                        .ThenInclude(n => n.User)
                                        .Where(n => n.Patient.UserId == user.Id).ToList();


                ViewBag.Consult = _context.Consultations.Include(c => c.Patient).ThenInclude(c => c.User)
                                                        .Include(c => c.Doctor).ThenInclude(c => c.User)
                                                        .Include(c=>c.Visa)
                                                        .Where(c=>c.Patient.UserId == user.Id && c.IsSubmitted == true).ToList();
                var visa = _context.Visas.Include(v => v.Patient).Where(v => v.Patient.UserId == user.Id).FirstOrDefault();

                if (visa != null)
                {
                    ViewBag.VisaNumber = visa.VisaNumber;
                    ViewBag.ExpDate = visa.ExpDate;
                }

                List<Patient> users = new List<Patient>();
                users.AddRange(_context.Patients);

                foreach (var existingUser in users)
                {
                    if (user.Email == existingUser.User?.Email && user.UserName == existingUser.User?.UserName)
                    {
                        return View(existingUser);
                    }
                }

                return View();

            }


            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region EditProfile
        [HttpGet]
        public IActionResult EditProfile(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return RedirectToAction(nameof(Profile));
            }
            var user = _userManager.GetUserAsync(User).Result;

            if (user != null)
            {
                ViewBag.NoteResponse = _context.DoctorNoteToPatients.Include(n => n.Doctor)
                                         .ThenInclude(n => n.User)
                                         .Include(n => n.Consultation).Include(n => n.Patient)
                                         .ThenInclude(n => n.User)
                                         .Where(n => n.Patient.UserId == user.Id).ToList();


                ViewBag.Consult = _context.Consultations.Include(c => c.Patient).ThenInclude(c => c.User)
                                                       .Include(c => c.Doctor).ThenInclude(c => c.User)
                                                       .Include(c => c.Visa)
                                                       .Where(c => c.Patient.UserId == user.Id && c.IsSubmitted == true).ToList();

                var visa = _context.Visas.Include(v => v.Patient).Where(v => v.Patient.UserId == user.Id).FirstOrDefault();

                if (visa != null)
                {
                    ViewBag.VisaNumber = visa.VisaNumber;
                    ViewBag.ExpDate = visa.ExpDate;
                }
                ViewBag.Visa = visa;
                List<Patient> users = new List<Patient>();
                users.AddRange(_context.Patients);

                foreach (var existingUser in users)
                {
                    if (user.Email == existingUser.User?.Email && user.UserName == existingUser.User?.UserName)
                    {
                        return View(existingUser);
                    }
                }

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Patient model, IFormFile? FormFile)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    ViewBag.NoteResponse = _context.DoctorNoteToPatients.Include(n => n.Doctor)
                                        .ThenInclude(n => n.User)
                                        .Include(n => n.Consultation).Include(n => n.Patient)
                                        .ThenInclude(n => n.User)
                                        .Where(n => n.Patient.UserId == user.Id).ToList();

                    ViewBag.Consult = _context.Consultations.Include(c => c.Patient).ThenInclude(c => c.User)
                                                                         .Include(c => c.Doctor).ThenInclude(c => c.User)
                                                                         .Include(c => c.Visa)
                                                                         .Where(c => c.Patient.UserId == user.Id && c.IsSubmitted == true).ToList();
                    var visa = _context.Visas.Include(v => v.Patient).Where(v => v.Patient.UserId == user.Id).FirstOrDefault();

                    if (visa != null)
                    {
                        ViewBag.VisaNumber = visa.VisaNumber;
                        ViewBag.ExpDate = visa.ExpDate;
                    }
                    ViewBag.Visa = visa;
                    var patient = await _context.Patients.FindAsync(model.PatientID);

                    if (patient != null)
                    {
                        patient.User.UserName = model.User?.UserName;
                        patient.User.NormalizedUserName = model.User?.UserName.ToUpper();
                        patient.User.FirstName = model.User?.FirstName;
                        patient.User.LastName = model.User?.LastName;
                        patient.User.PhoneNumber = model.User?.PhoneNumber;
                        patient.Patient_Age = model.Patient_Age;

                        if (FormFile != null)
                        {
                            using (var stream = FormFile.OpenReadStream())
                            using (var reader = new BinaryReader(stream))
                            {
                                var byteFile = reader.ReadBytes((int)stream.Length);
                                patient.Image = byteFile;
                            }
                            patient.ImageName = FormFile.FileName;
                            patient.ContentType = FormFile.ContentType;
                        }
                        _context.Update(patient);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Profile));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Patient not found.");
                    }


                }
            }
            return View(model);
        }
        #endregion
    }
}
