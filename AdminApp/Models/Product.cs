using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminApp.Models
{
    public class Product
    {

        [Key]
        public int ProductId { get; set; }
    
        [Required]
        [StringLength(150)]
        public string ProductName { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public int StockQty { get; set; } = 0;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
