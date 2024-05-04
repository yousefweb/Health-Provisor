using HealthProvisor.Data.Enum;
using HealthProvisor.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Doctor
{
	[Key]
	public int DoctorID { get; set; }

	public string? UserId { get; set; }
	public virtual User? User { get; set; }

    public int ?CategoryId { get; set; }
    public virtual Category ?Category { get; set; }

    [Required(ErrorMessage = "Please enter your specialization.")]
    public MedicalSpecialization Doctor_Specialization { get; set; }


    [Required(ErrorMessage = "Please enter the doctor's age.")]
    [Range(26, 100, ErrorMessage = "Age must be between 26 and 100.")]
    public int Doctor_Age { get; set; }

    [Required]
    [Display(Name = "Gender")]
    public Gender Doctor_Gender { get; set; }

    [Required(ErrorMessage = "Please enter the years of experience.")]
    [Range(1, int.MaxValue, ErrorMessage = "Years of experience must be a non-negative number.")]
    public int Doctor_YearsOfExperience { get; set; }

    public string? DoctorStatus { get; set; }

    [StringLength(255, ErrorMessage = "Image name length must be less than or equal to 255 characters")]
    public string? ImageName { get; set; }

    [StringLength(10, ErrorMessage = "Content type length must be less than or equal to 50 characters")]
    public string? ContentType { get; set; }

    [Required(ErrorMessage = "Image is required")]
    public byte[]? Image { get; set; }

    [StringLength(255, ErrorMessage = "Certificate name length must be less than or equal to 255 characters")]
    public string? CertificateName { get; set; }

    [StringLength(10, ErrorMessage = "Content type length must be less than or equal to 50 characters")]
    public string? CertificateContentType { get; set; }

    [Required(ErrorMessage = "Certificate is required")]
    public byte[]? Certificate { get; set; }

    public ICollection<Consultation> ?Consultations { get; set; }
    public ICollection<DoctorNoteToPatient> ? DoctorNoteToPatients { get; set; }
}
