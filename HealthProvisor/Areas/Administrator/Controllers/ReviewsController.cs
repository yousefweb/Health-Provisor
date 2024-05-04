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
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Administrator/Reviews
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reviews.Include(r => r.Consultation).Include(r => r.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Administrator/Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Consultation)
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Administrator/Reviews/Create
        public IActionResult Create()
        {
            ViewData["ConsultationID"] = new SelectList(_context.Consultations, "ConsultationID", "Notes");
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID");
            return View();
        }

        // POST: Administrator/Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,ReviewRate,ReviewMessage,ReviewDate,PatientID,ConsultationID")] Review review)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var patient = _context.Patients.FirstOrDefault(p => p.UserId == user.Id);

                    if (patient != null)
                    {
                        review.PatientID = patient.PatientID;

                        if (review.ConsultationID > 0)
                        {
                            var isConsultationValid = _context.Consultations
                                .Any(c => c.ConsultationID == review.ConsultationID && c.PatientID == patient.PatientID);

                            if (!isConsultationValid)
                            {
                                ModelState.AddModelError(string.Empty, "Invalid consultation for the patient.");
                                return View(review);
                            }

                            review.ReviewStatus = "Pending";

                            _context.Add(review);
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Consultation ID is required.");
                        }
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "User not found, not authenticated, or patient not found.");
            return View(review);
        }



        // GET: Administrator/Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["ConsultationID"] = new SelectList(_context.Consultations, "ConsultationID", "Notes", review.ConsultationID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", review.PatientID);
            return View(review);
        }

        // POST: Administrator/Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,ReviewRate,ReviewMessage,ReviewDate,ConsultationID,PatientID")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
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
            ViewData["ConsultationID"] = new SelectList(_context.Consultations, "ConsultationID", "Notes", review.ConsultationID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", review.PatientID);
            return View(review);
        }

        // GET: Administrator/Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Consultation)
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Administrator/Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reviews == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reviews'  is null.");
            }
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ReviewsRequest()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.Users.Where(user => user.Id == Id).SingleOrDefault();
            ViewBag.Reviews = _context.Reviews.Include(r => r.Patient.User).Include(r => r.Consultation).ToList();
            return View(user);
        }
        public async Task<IActionResult> EditReviewStatus(int ReviewId, string newStatus)
        {
            var review = await _context.Reviews.FindAsync(ReviewId);
            review.ReviewStatus = newStatus;
            _context.Update(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ReviewsRequest));
        }
        private bool ReviewExists(int id)
        {
            return (_context.Reviews?.Any(e => e.ReviewId == id)).GetValueOrDefault();
        }

    }
}
