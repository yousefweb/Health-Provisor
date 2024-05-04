using HealthProvisor.Data.Enum;
using HealthProvisor.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class Patient
{
    [Key]
    public int PatientID { get; set; }

    public string? UserId { get; set; }
    public virtual User? User { get; set; }

    [Required(ErrorMessage = "Age is required")]
    [Range(16, 100, ErrorMessage = "Age must be between 16 and 100")]
    [Display(Name = "Age")]
    public int Patient_Age { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    [Display(Name = "Gender")]
    public Gender Patient_Gender { get; set; }

    public string?Patient_Status { get; set; }

    [StringLength(255, ErrorMessage = "Image name length must be less than or equal to 255 characters")]
    public string? ImageName { get; set; }

    [StringLength(10, ErrorMessage = "Content type length must be less than or equal to 50 characters")]
    public string? ContentType { get; set; }


    public byte[]? Image { get; set; }
    
    public ICollection<Consultation>? Consultations { get; set; }
    public ICollection<DoctorNoteToPatient>? DoctorNoteToPatients { get; set; }

}

