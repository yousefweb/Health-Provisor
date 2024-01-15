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
namespace HealthProvisor.Controllers
{
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

        // GET: Patients
        public async Task<IActionResult> Index()
        {
              return _context.Patients != null ? 
                          View(await _context.Patients.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Patients'  is null.");
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
			ViewBag.gender = Enum.GetValues(typeof(Gender));
			return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
     
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
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true,
                    NormalizedEmail = model.Email?.ToUpper(),
                    NormalizedUserName = model.UserName?.ToUpper(),
                    Role = "PATIENT"
                };
                if (user != null && await _userManager.IsInRoleAsync(user, "PATIENT") && await _userManager.CheckPasswordAsync(user, model.Password.ToString()))
                { }
                    var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var patient = new Patient
                    {
                        User = user,
                        Patient_Age = model.Age,
                        Patient_Gender = model.Gender,
                        Patient_Status="Pending"
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
      


        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
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

        private bool PatientExists(int id)
        {
          return (_context.Patients?.Any(e => e.PatientID == id)).GetValueOrDefault();
        }

    }
}
