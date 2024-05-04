using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthProvisor.Data;
using HealthProvisor.Models.ViewModel;
using HealthProvisor.Models;
using Microsoft.AspNetCore.Identity;

namespace HealthProvisor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<APIController> _logger;

        public APIController(ApplicationDbContext context, UserManager<User> userManager, ILogger<APIController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostCreatePatient([FromForm] PatientRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.UserName == model.UserName))
                {
                    return BadRequest("UserName is already taken");
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
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
                    return Ok("User and patient created successfully.");
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error during user creation: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest(ModelState);
        }
    }
}
