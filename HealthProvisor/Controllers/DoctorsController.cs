using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using HealthProvisor.Models.ViewModel;
using HealthProvisor.Models;
using Microsoft.AspNetCore.Identity;
using System.Numerics;
using HealthProvisor.Data.Enum;
using Microsoft.AspNetCore.Authorization;
using MimeKit;
using SQLitePCL;

namespace HealthProvisor.Controllers
{
    [Authorize(Roles = "DOCTOR")]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;


        public DoctorsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Doctors.Include(d => d.Category).Include(d => d.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Doctors/Details/5
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

        // GET: Doctors/Create
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create()
        {
            ViewBag.Gender = Enum.GetValues(typeof(Gender));
            ViewBag.Specialization = Enum.GetValues(typeof(MedicalSpecialization));

            var categories = _context.Categories.ToList();

            var viewModel = new DoctorRegisterViewModel
            {
                Categories = categories,
                DoctorStatus = "Pending"
            };

            return View(viewModel);
        }


        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(DoctorRegisterViewModel model)
        {
           
            if (!ModelState.IsValid)
            {
                model.Categories = _context.Categories.ToList();

                return View(model);
            }


            if (_context.Users.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Username is already taken");
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                NormalizedEmail = model.Email?.ToUpper(),
                NormalizedUserName = model.UserName?.ToUpper(),
                Role = "DOCTOR"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var doctor = new Doctor
                {
                    User = user,
                    Doctor_Specialization = model.Doctor_Specialization,
                    Doctor_Age = model.Doctor_Age,
                    Doctor_Gender = model.Doctor_Gender,
                    Doctor_YearsOfExperience = model.Doctor_YearsOfExperience,
                    CategoryId = model.CategoryId,
                    DoctorStatus = "Pending"
                };


                using (var stream = model.FormFile?.OpenReadStream())
                using (var reader = new BinaryReader(stream))
                {
                    var byteFile = reader.ReadBytes((int)stream.Length);
                    doctor.Image = byteFile;
                }

                doctor.ImageName = model.FormFile?.FileName;
                doctor.ContentType = model.FormFile?.ContentType;

                using (var stream = model.FormFile2?.OpenReadStream())
                using (var reader = new BinaryReader(stream))
                {
                    var byteFile = reader.ReadBytes((int)stream.Length);
                    doctor.Certificate = byteFile;
                }

                doctor.CertificateName = model.FormFile2?.FileName;
                doctor.CertificateContentType = model.FormFile2?.ContentType;


                _context.Doctors.Add(doctor);
            
                

                var roleId = "2";
                var userRole = new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = roleId
                };
               

                _context.UserRoles.Add(userRole);                

                await _context.SaveChangesAsync();

                await SendDoctorStatusConfirmationEmail(doctor);

                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);

        }


        // GET: Doctors/Edit/5
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

        // POST: Doctors/Edit/5
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

        // GET: Doctors/Delete/5
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

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Doctors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Doctors'  is null.");
            }
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return (_context.Doctors?.Any(e => e.DoctorID == id)).GetValueOrDefault();
        }

        [HttpGet]
        public IActionResult DoctorProfile() // Dashboard
        {
            return View();
        }

        #region Profile
        public IActionResult Profile()
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user != null)
            {

                var doctorId = _context.Doctors
                               .Where(d => d.UserId == user.Id)
                               .Select(d => d.DoctorID)
                               .FirstOrDefault();


                if (doctorId != null)
                {

                    var consultationsA = _context.Consultations
                        .Include(c => c.Patient)
                        .ThenInclude(p => p.User)
                        .Where(c => c.DoctorID == doctorId && c.Patient_Status == "Accept" && c.IsSubmitted == true)
                        .ToList();

                    var consultationsC = _context.Consultations
                      .Include(c => c.Patient)
                      .ThenInclude(p => p.User)
                      .Where(c => c.DoctorID == doctorId && c.Patient_Status == "Completed" && c.IsSubmitted == true)
                      .ToList();


                    if (consultationsA.Any())
                    {

                       
                        ViewBag.PatientCountA = consultationsA.Count();
                        ViewBag.Patients = consultationsA.Select(c => c.FirstName + " " + c.LastName).ToList();
                    }
                    if (consultationsC.Any())
                    {

                        ViewBag.PatientCountC = consultationsC.Count();
                      
                    }
                    if (!consultationsA.Any() && !consultationsC.Any())
                    {
                        ViewBag.PatientCountA = 0;
                        ViewBag.PatientCountC = 0;
                        ViewBag.Patients = new List<string>();
                    }
                    List<Doctor> users = new List<Doctor>();
                    users.AddRange(_context.Doctors);
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


            return View();
        }

        #endregion

        #region EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return RedirectToAction(nameof(Profile));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var doctorId = _context.Doctors
                            .Where(d => d.UserId == currentUser.Id)
                            .Select(d => d.DoctorID)
                            .FirstOrDefault();

                if (doctorId != null)
                {
                    var consultationsA = _context.Consultations
                          .Include(c => c.Patient)
                          .ThenInclude(p => p.User)
                          .Where(c => c.DoctorID == doctorId && c.Patient_Status == "Accept" && c.IsSubmitted == true)
                          .ToList();

                    var consultationsC = _context.Consultations
                      .Include(c => c.Patient)
                      .ThenInclude(p => p.User)
                      .Where(c => c.DoctorID == doctorId && c.Patient_Status == "Completed" && c.IsSubmitted == true)
                      .ToList();



                    if (consultationsA.Any())
                    {
                        ViewBag.PatientCountA = consultationsA.Count();
                        ViewBag.Patients = consultationsA.Select(c => c.FirstName + " " + c.LastName).ToList();
                    }
                    if (consultationsC.Any())
                    {
                        ViewBag.PatientCountC = consultationsC.Count();
                    }
                    if (!consultationsA.Any() && !consultationsC.Any())
                    {
                        ViewBag.PatientCountA = 0;
                        ViewBag.PatientCountC = 0;
                        ViewBag.Patients = new List<string>();
                    }

                    List<Doctor> users = new List<Doctor>();
                    users.AddRange(_context.Doctors);
                    foreach (var existingUser in users)
                    {
                        if (currentUser.Email == existingUser.User?.Email && currentUser.UserName == existingUser.User?.UserName)
                        {
                            return View(existingUser);
                        }
                    }
                }
                return View();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Doctor model, IFormFile ?FormFile)
        {
           
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var doctorId = _context.Doctors
                                .Where(d => d.UserId == currentUser.Id)
                                .Select(d => d.DoctorID)
                                .FirstOrDefault();

                    if (doctorId != null)
                    {
                        var consultationsA = _context.Consultations
                                            .Include(c => c.Patient)
                                            .ThenInclude(p => p.User)
                                            .Where(c => c.DoctorID == doctorId && c.Patient_Status == "Accept" && c.IsSubmitted == true)
                                            .ToList();

                        var consultationsC = _context.Consultations
                          .Include(c => c.Patient)
                              .ThenInclude(p => p.User)
                          .Where(c => c.DoctorID == doctorId && c.Patient_Status == "Completed" && c.IsSubmitted == true)
                          .ToList();



                        if (consultationsA.Any())
                        {
                            ViewBag.PatientCountA = consultationsA.Count();
                            ViewBag.Patients = consultationsA.Select(c => c.FirstName + " " + c.LastName).ToList();
                        }
                        if (consultationsC.Any())
                        {
                            ViewBag.PatientCountC = consultationsC.Count();
                        }
                        if (!consultationsA.Any() && !consultationsC.Any())
                        {
                            ViewBag.PatientCountA = 0;
                            ViewBag.PatientCountC = 0;
                            ViewBag.Patients = new List<string>();
                        }

                    }

                    var doctor = await _context.Doctors.FindAsync(model.DoctorID);

                    if (doctor != null)
                    {
                        doctor.User.UserName = model.User?.UserName;
                        doctor.User.NormalizedUserName = model.User?.UserName.ToUpper();
                        doctor.User.FirstName = model.User?.FirstName;
                        doctor.User.LastName = model.User?.LastName;
                        doctor.User.PhoneNumber = model.User?.PhoneNumber;
                        doctor.Doctor_Age = model.Doctor_Age;
                        doctor.Doctor_YearsOfExperience = model.Doctor_YearsOfExperience;

                        if (FormFile != null)
                        {
                            using (var stream = FormFile.OpenReadStream())
                            using (var reader = new BinaryReader(stream))
                            {
                                var byteFile = reader.ReadBytes((int)stream.Length);
                                doctor.Image = byteFile;
                            }
                            doctor.ImageName = FormFile.FileName;
                            doctor.ContentType = FormFile.ContentType;

                        }


                        _context.Update(doctor);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Profile));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Doctor not found.");
                    }

                }
            }

            return View(model);
        }

        #endregion



        public async Task SendDoctorStatusConfirmationEmail(Doctor doctor)
        {

            Doctor doc = _context.Doctors.Include(d => d.User).FirstOrDefault(x => x.DoctorID == doctor.DoctorID);
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("HealthProvisor", "HealthProvisor@gmail.com"));
            email.To.Add(new MailboxAddress($"{doc.User?.FirstName} {doc.User?.LastName}", doc.User?.Email));
            email.Subject = "Status Confirmation";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Dear {doc.User?.FirstName} {doc.User?.LastName},\n\n"
                             + "Thank you for choosing us!\n\n"
                             + "Your Status has been Confirmed.\n"
                             + "Wait for the Owner to check your info and if you are accepted then you will recieve an email\n"
                             + "We appreciate your taking a time for work with us and look forward to serving you again!\n"
                             + "Best regards,\n"
                             + "Health Provisor Team";

            email.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate("HealthProvisor@gmail.com", "zfii pcdg ysyy aunn");
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
        }
    }
}
