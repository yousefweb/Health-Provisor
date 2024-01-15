using HealthProvisor.Data.Enum;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace HealthProvisor.Models.ViewModel
{
    public class DoctorRegisterViewModel
    {
        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public List<Category> Categories { get; set; }

        [Required(ErrorMessage = "Please enter your username.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your first name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your phone number.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the doctor's age.")]
        [Range(26, 100, ErrorMessage = "Age must be between 26 and 100.")]
        public int Doctor_Age { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public Gender Doctor_Gender { get; set; }

        [Required(ErrorMessage = "Please enter your specialization.")]
        public MedicalSpecialization? Doctor_Specialization { get; set; }


        [Required(ErrorMessage = "Please enter the years of experience.")]
        [Range(1, int.MaxValue, ErrorMessage = "Years of experience must be a non-negative number.")]
        public int Doctor_YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Doctor status is required")]
        public string DoctorStatus { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile FormFile { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}