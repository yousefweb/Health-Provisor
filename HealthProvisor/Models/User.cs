using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthProvisor.Models
{
    public class User : IdentityUser
    {
		[Required(ErrorMessage = "First name is required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; }


		public virtual Doctor? Doctor { get; set; }
		public virtual Patient? Patient { get; set; }

	}
}
