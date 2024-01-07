using System;
using System.ComponentModel.DataAnnotations;

namespace _5Dots.Models
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
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpDate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Transaction date is required")]
        [DataType(DataType.DateTime)]
        public DateTime TransactionDate { get; set; }

        [Required]
        public int PatientID { get; set; }

        public Patient Patient { get; set; }
    }
}
