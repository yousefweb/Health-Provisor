using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name must not exceed 100 characters")]
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }
}
