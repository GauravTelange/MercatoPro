using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MercatoAPI.Models  
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage= " Only letters allowed")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile must be 10 digits")]
        public string Mobile { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$",
            ErrorMessage = "Password must have uppercase, lowercase and number")]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string Sex { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s,.\-\/#]+$", ErrorMessage = "Address contains invalid characters")]
        public string Address { get; set; }

        public string Role { get; set; } = "Customer";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}