using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using HealthProvisor.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace HealthProvisor.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class TestimonialsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TestimonialsController(ApplicationDbContext context,UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Administrator/Testimonials
        public async Task<IActionResult> Index()
        {
            var testimonials = _context.Testimonials
        .Include(t => t.Patient)
        .ThenInclude(p => p.User);

            return View(await testimonials.ToListAsync());
        }

        // GET: Administrator/Testimonials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(m => m.TestimonialID == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // GET: Administrator/Testimonials/Create
        public IActionResult Create()
        {
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID");
            return View();
        }

        // POST: Administrator/Testimonials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Testimonial testimonial)
        {

            if (User.Identity.IsAuthenticated)
            {

                var user = await _userManager.GetUserAsync(User);


                if (user != null)
                {

                    var patient = _context.Patients.FirstOrDefault(p => p.UserId == user.Id);


                    if (patient != null)
                    {
                        testimonial.PatientID = patient.PatientID;

                        testimonial.TestimonialStatus = "Pending";


                        _context.Add(testimonial);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            // Handle the case where the user is not authenticated, not found, or patient not found
            ModelState.AddModelError(string.Empty, "User not found, not authenticated, or patient not found.");
            return View(testimonial);
        }

        // GET: Administrator/Testimonials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", testimonial.PatientID);
            return View(testimonial);
        }

        // POST: Administrator/Testimonials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Testimonial testimonial)
        {
            if (id != testimonial.TestimonialID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(testimonial.TestimonialID))
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
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", testimonial.PatientID);
            return View(testimonial);
        }

        // GET: Administrator/Testimonials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(m => m.TestimonialID == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: Administrator/Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Testimonials == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Testimonials'  is null.");
            }
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestimonialExists(int id)
        {
          return (_context.Testimonials?.Any(e => e.TestimonialID == id)).GetValueOrDefault();
        }
        public IActionResult TestimonialsRequest()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.Users.Where(user => user.Id == Id).SingleOrDefault();
            ViewBag.Testimonials = _context.Testimonials.Include(testimonial => testimonial.Patient.User).ToList();
            return View(user);
        }
        public async Task<IActionResult> EditTestimonialStatus(int TestimonialID, string newStatus)
        {
            var testimonial = await _context.Testimonials.FindAsync(TestimonialID);
            testimonial.TestimonialStatus = newStatus;
            _context.Update(testimonial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TestimonialsRequest));
        }
    }
}
