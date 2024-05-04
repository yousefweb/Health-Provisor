using HealthProvisor.Data;
using HealthProvisor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HealthProvisor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
        }
        public IActionResult Index(string selectedCategory)
        {
            ViewBag.Categories = _context.Categories.Include(category => category.Doctors).ToList();
            ViewBag.SelectedCategory = selectedCategory; 
            ViewBag.Testimonials = _context.Testimonials
                .Include(testimonial => testimonial.Patient)
                .Where(testimonial => testimonial.TestimonialStatus == "Accept")
                .OrderBy(testimonial => testimonial.Patient.User.Id)
                .Take(3)
                .ToList();

            ViewBag.Doctors = _context.Doctors.Include(doc => doc.Consultations).Where(docSt => docSt.DoctorStatus == "Accept").ToList();

            return View();
        }


        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult OurDoctor()
        {
			var doctors = _context.Doctors.Include(d => d.Category).Include(d => d.User).Where(d => d.DoctorStatus == "Accept").ToList();
			ViewBag.Doctors = doctors;
			return View();
        }
        public IActionResult Blog()
        {
            return View();
        }

		public IActionResult Services()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
     
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
