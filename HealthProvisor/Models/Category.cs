using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthProvisor.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name must not exceed 100 characters")]
        public string ?CategoryName { get; set; }

        public string ?CategoryDescription { get; set; }

        public string ?BenefitsHeading { get; set; }

        [NotMapped] 
        [DisplayName("Image")]
        public IFormFile ?FormFile { get; set; }

        [StringLength(255, ErrorMessage = "Image name length must be less than or equal to 255 characters")]
        public string ?ImageName { get; set; }

        [StringLength(10, ErrorMessage = "Content type length must be less than or equal to 50 characters")]
        public string ?ContentType { get; set; }

        public byte[] ?Image { get; set; }
        public ICollection<Doctor>? Doctors { get; set; }
    }
}
