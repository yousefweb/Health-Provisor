using HealthProvisor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthProvisor.Areas.Administrator.Controllers
{      
    [Authorize(Roles = "ADMIN")] 
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
		public AdminController(ApplicationDbContext context)
		{
			_context = context;
		}
        [Area("Administrator")]
        public IActionResult Index()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.Users.Where(user => user.Id == Id).SingleOrDefault();
			ViewBag.Users = _context.Users.Where(user => user.Role == "User").ToList();
			ViewBag.Categories = _context.Categories.ToList();
			ViewBag.Patients = _context.Patients.ToList();
			ViewBag.Doctors = _context.Doctors.Include(x =>x.Category).ToList();
            return View(User);
        }

    }
}
