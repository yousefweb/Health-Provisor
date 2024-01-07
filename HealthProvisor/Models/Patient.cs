using HealthProvisor.Data.Enum;
using HealthProvisor.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


public class Patient
{

	[Key]
	public int PatientID { get; set; }

	[Required]
	public string UserId { get; set; }
	public virtual User User { get; set; }

	[Required(ErrorMessage = "PatientAge is required")]
	[Display(Name = "Age")]
	public int Patient_Age { get; set; }

	[Required]
	[Display(Name = "Gender")]
	public Gender Patient_Gender { get; set; }

	[StringLength(255, ErrorMessage = "Image name length must be less than or equal to 255 characters")]
	public string? ImageName { get; set; }

	[StringLength(10, ErrorMessage = "Content type length must be less than or equal to 50 characters")]
	public string? ContentType { get; set; }

	[Required(ErrorMessage = "Image is required")]
	public byte[]? Image { get; set; }

	public ICollection<Consultation> Consultations { get; set; }
}
