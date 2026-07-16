using System.ComponentModel.DataAnnotations;

namespace MercatoAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        public string Description { get; set; }
    }
}
