using System;
using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models
{
    public class Visa
    {
        [Key]
        public int VisaId { get; set; }

        [Required(ErrorMessage = "CVC is required")]
        [Range(100, 999, ErrorMessage = "CVC must be a 3-digit number")]
        public short CVC { get; set; }

        [Required(ErrorMessage = "Visa number is required")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Visa number must be exactly 16 characters")]
        public string VisaNumber { get; set; }

        [Required(ErrorMessage = "Expiration date is required")]
        [Display(Name = "Expiration Date")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Please enter a valid expiration date in the format MM/yy.")]
        public string ExpDate { get; set; }


        [Required(ErrorMessage = "Transaction date is required")]
        [DataType(DataType.DateTime)]
        public DateTime TransactionDate { get; set; }

        [Required]
        public int PatientID { get; set; }

        public Patient ?Patient { get; set; }
        public ICollection<Consultation>? Consultations { get; set; }

        public bool Equalss(Visa other)
        {
            if (other == null)
                return false;

            return this.CVC == other.CVC &&
                   this.VisaNumber == other.VisaNumber &&
                   this.ExpDate == other.ExpDate &&
                   this.PatientID == other.PatientID;
        }
    }
}
