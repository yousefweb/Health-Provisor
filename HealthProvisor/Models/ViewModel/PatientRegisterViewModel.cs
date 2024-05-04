using HealthProvisor.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models.ViewModel
{
    public class PatientRegisterViewModel
    {

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only alphabetical characters are allowed for the first name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only alphabetical characters are allowed for the Last name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(16, 100, ErrorMessage = "Age must be between 16 and 100")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        public string?Patient_Status { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please confirm your password.")]
        public string ConfirmPassword { get; set; }

       
        [Required(ErrorMessage = "Image is required")]
        public IFormFile FormFile { get; set; }
    }
}
