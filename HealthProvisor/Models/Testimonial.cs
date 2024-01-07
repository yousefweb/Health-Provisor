using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models
{
    public class Testimonial
    {
        [Key]
        public int TestimonialID { get; set; }

        [Required(ErrorMessage = "Please enter the testimonial date.")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Testimonial status is required")]
        public string TestimonialStatus { get; set; }

        [Required(ErrorMessage = "Testimonial message is required")]
        public string TestimonialMessage { get; set; }

        [Required]
        public int PatientID { get; set; }
        public Patient Patient { get; set; }
    }
}
