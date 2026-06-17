using System.ComponentModel.DataAnnotations;

namespace MercatoPro.Models  
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string Mobile { get; set; }

        [Required]
        public string Password { get; set; }

        public string Sex { get; set; }

        public string Address { get; set; }

        public string Role { get; set; } = "Customer";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}