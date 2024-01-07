using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Range(0, 5, ErrorMessage = "Review rate must be between 0 and 5")]
        public int ReviewRate { get; set; }

        [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string ReviewMessage { get; set; }

        [Required(ErrorMessage = "Review date is required")]
        [DataType(DataType.DateTime)]
        public DateTime ReviewDate { get; set; }

        [Required(ErrorMessage = "Consultation ID is required")]
        public int ConsultationID { get; set; }

        public Consultation Consultation { get; set; }

        
        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientID { get; set; }

        public Patient Patient { get; set; }
    }
}
