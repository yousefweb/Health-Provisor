using HealthProvisor.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models
{
	public class Consultation
	{
		[Key]
		public int ConsultationID { get; set; }

		public int PatientID { get; set; }
		public Patient ?Patient { get; set; }

		public int DoctorID { get; set; }
		public Doctor ?Doctor { get; set; }

        public int VisaId { get; set; } 
        public Visa ?Visa { get; set; } 


        [Required(ErrorMessage = "First name is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only alphabetical characters are allowed for the first name.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only alphabetical characters are allowed for the Last name.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(16, 100, ErrorMessage = "Age must be between 16 and 100")]
        [Display(Name = "Age")]
        public int Patient_Age { get; set; }

        public string? Patient_Status { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public Gender Patient_Gender { get; set; }

        [Required(ErrorMessage = "Please enter the consultation date.")]
		[DataType(DataType.DateTime)]
		public DateTime Date { get; set; }

		[Required(ErrorMessage = "Please enter consultation notes.")]
        public string ?Notes { get; set; }

        public bool IsSubmitted { get; set; }
        public ICollection<DoctorNoteToPatient>? DoctorNoteToPatients { get; set; }

    }


}
