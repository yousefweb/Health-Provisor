using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models
{
    public class DoctorNoteToPatient
    {
        
        public int Id { get; set; }

        public string ?ConsultationDetails { get; set; }

        [Required(ErrorMessage = "Doctor note is required.")]
        public string ?DoctorNote { get; set; }

        [Required(ErrorMessage = "Message time is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime MessageTime { get; set; }

        public int PatientID { get; set; }
        public Patient ?Patient { get; set; }

        public int ConsultationID { get; set; }
        public Consultation ?Consultation { get; set; }

        public int DoctorID { get; set; }
        public Doctor ?Doctor { get; set; }
    }
}
