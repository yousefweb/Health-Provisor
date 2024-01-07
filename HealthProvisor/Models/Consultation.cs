using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models
{
	public class Consultation
	{
		[Key]
		public int ConsultationID { get; set; }

		[Required]
		public int PatientID { get; set; }
		public Patient Patient { get; set; }

		[Required]
		public int DoctorID { get; set; }
		public Doctor Doctor { get; set; }

		[Required(ErrorMessage = "Please enter the consultation date.")]
		[DataType(DataType.DateTime)]
		public DateTime Date { get; set; }

		[Required(ErrorMessage = "Please enter consultation notes.")]
		[MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
		public string Notes { get; set; }
	}


}
